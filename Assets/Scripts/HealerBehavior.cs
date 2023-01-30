using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HealerBehavior : MonoBehaviour
{
    public Enemy enemyScript;
    public Health enemyHealth;
    public string healAnimation;
    public string afterHealAnimation;
    public GameObject healParticles;
    public float cooldown;
    public float cooldownDefault;
    public float healDuration;
    public GameObject soundPerformHeal;
    public GameObject soundHealed;
    public GameObject soundRevived;

    Animator spriteAnimator;
    List<GameObject> healTargets = new List<GameObject>();

    void Start()
    {
        spriteAnimator = enemyScript.sprite.GetComponent<Animator>();
        StartCoroutine(DecideToHeal());
    }

    void Update()
    {
        // Cooldown timer. Must wait "cooldownDefault" seconds before being able to heal again.
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        if(cooldown < 0)
        {
            cooldown = 0;
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Every enemy that gets close to this one is added to a list. The enemy that heals can't heal itself. Targets can't be added twice to the list.
        if (EnemyIsActive() && other.gameObject.tag != "Player" && other.gameObject.GetComponent<Enemy>() != null && other.gameObject != transform.parent.gameObject && healTargets.Contains(other.gameObject) == false)
        {
            // Adds to targets list if the target can be healed or if the target is dead and it can be revived.
            if (other.gameObject.GetComponent<Enemy>().canBeHealed == true || (other.gameObject.GetComponent<Health>().isDead == true && other.gameObject.GetComponent<Enemy>().canBeRevived == true))
            {
                healTargets.Add(other.gameObject);
            }
        }
    }

    // Occasionally check if should heal. Can't heal if the healTargets list is empty.
    IEnumerator DecideToHeal()
    {
        yield return new WaitForSeconds(0.5f);

        if(EnemyIsActive() && cooldown <= 0 && healTargets.Count > 0)
        {
            StartCoroutine(Heal(healDuration));
        }

        StartCoroutine(DecideToHeal());
    }

    // Performs the healing and reviving to every healTarget at once. This lasts for "t" seconds.
    public IEnumerator Heal(float t)
    {
        // Play healing animation and prevent enemy from attacking while healing.
        if (enemyHealth.isDead == false)
        {
            spriteAnimator.Play(healAnimation);
            enemyScript.wakeUpTimer = t;
        }

        // Don't let the sounds be too loud by limiting the amount of sound objects created.
        int snd_heal_limit = 3;
        int snd_revive_limit = 3;

        // Heal every target.
        foreach (GameObject target in healTargets)
        {
            Health targetScript = target.GetComponent<Health>();

            // If target is alive or can be revived.
            if ((targetScript.isDead == true && target.GetComponent<Enemy>().canBeRevived == false) == false)
            {
                // Revive or heal. Can't do both at the same time to the same target.
                if (targetScript.isDead == true && target.GetComponent<Enemy>().canBeRevived == true)
                {
                    target.GetComponent<Enemy>().Revive();

                    // Revived sound at target position.
                    if (soundRevived != null && snd_revive_limit > 0)
                    {
                        Instantiate(soundRevived, transform.position, transform.rotation);
                        snd_revive_limit--;
                    }
                }
                else if (targetScript.isDead == false)
                {
                    // Apply health to target.
                    if (targetScript.health <= targetScript.startHealth / 2)
                    {
                        targetScript.health = Mathf.RoundToInt(targetScript.health * 1.8f);
                    }
                    if (targetScript.health <= targetScript.startHealth / 4)
                    {
                        targetScript.health = Mathf.RoundToInt(targetScript.health * 2f);
                    }

                    // Apply armor to target.
                    if (targetScript.armor < 40)
                    {
                        targetScript.armor = 40;
                    }
                    if (targetScript.armorMult > 0.25f || targetScript.armorMult == 0.0f || targetScript.armorMult == 1.0f)
                    {
                        targetScript.armorMult = 0.25f;
                    }

                    // Healed sound at target position.
                    if (soundHealed != null && snd_heal_limit > 0)
                    {
                        Instantiate(soundHealed, transform.position, transform.rotation);
                        snd_heal_limit--;
                    }
                }

                // Create cosmetic effect at target position.
                if (healParticles != null)
                {
                    var particle = Instantiate(healParticles, target.transform.position + target.transform.forward, target.transform.rotation);
                    particle.transform.parent = target.transform;
                }
            }
        }

        if (soundPerformHeal != null)
        {
            Instantiate(soundPerformHeal, transform.position, transform.rotation);
        }

        yield return new WaitForSeconds(t);

        if (enemyHealth.isDead == false)
        {
            spriteAnimator.Play(afterHealAnimation);
        }

        enemyScript.attackTime += 1f;
        enemyScript.wakeUpTimer = 0;

        // Resets cooldown timer. Increase cooldown based on how many targets were healed.
        cooldown = cooldownDefault;
        cooldown += healTargets.Count;

        // Clears healTargets list.
        healTargets.Clear();
    }

    // Returns true if the enemy with the healer behaviour is not busy attacking, waking up, or dead or with no target.
    bool EnemyIsActive()
    {
        return enemyScript.attackTime > 0 && enemyScript.wakeUpTimer <= 0 && enemyHealth.isDead == false && enemyScript.target != null;
    }
}
