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
        if (EnemyIsActive() && other.gameObject.tag != "Player" && other.gameObject.GetComponent<Enemy>() != null && other.gameObject != transform.parent.gameObject && healTargets.Contains(other.gameObject) == false)
        {
            if (other.gameObject.GetComponent<Enemy>().canBeHealed == true)
            {
                healTargets.Add(other.gameObject);
            }
        }
    }

    IEnumerator DecideToHeal()
    {
        yield return new WaitForSeconds(0.5f);

        if(EnemyIsActive() && cooldown <= 0 && healTargets.Count > 0)
        {
            StartCoroutine(Heal(healDuration));
        }

        StartCoroutine(DecideToHeal());
    }

    public IEnumerator Heal(float t)
    {
        spriteAnimator.Play(healAnimation);
        enemyScript.wakeUpTimer = t;

        // Don't let the sounds be too loud.
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

        spriteAnimator.Play(afterHealAnimation);
        enemyScript.attackTime += 1f;
        enemyScript.wakeUpTimer = 0;

        cooldown = cooldownDefault;
        cooldown += healTargets.Count;

        healTargets.Clear();
    }

    bool EnemyIsActive()
    {
        return enemyScript.attackTime > 0 && enemyScript.wakeUpTimer <= 0 && enemyHealth.isDead == false && enemyScript.target != null;
    }
}
