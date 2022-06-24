using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject sprite;
    public GameObject target;

    // Score

    public int giveScoreOnDeath = 0;

    // Ranged attack

    public float attackTimeDefault;
    public float attackTime = 0;
    public GameObject attackProjectile;
    public string attackAnim;
    public float attackShotDelay = 0;
    public float attackTotalDuration;
    public GameObject attackSound;

    // Melee attack

    public string meleeAnim;
    public int meleeDamage;
    public float meleeRange;
    public float meleeStartDelay;
    public float meleeDuration;
    public GameObject meleeSound;

    // Attack types

    public bool hasRangedAttack = true;
    public bool hasMeleeAttack = false;

    public string deathAnim;
    bool attacking = false;
    bool died = false;

    public LayerMask sightMask;

    // Pain

    bool inPain = false;
    public float painTime = 0.5f;
    public int painChance = 0;
    public string painAnim;

    // Pain chance
    // 0 = Always
    // 1 = 50% chance
    // 2 = 33% chance
    // 3+ = Smaller chance

    public int previousPositionsItem = 0;
    public int previousPositionSelected = 0;
    public Vector3[] previousPositions = new Vector3[10];
    public bool goingToPreviousPosition = false;

    Vector3 dir;
    NavMeshAgent agent;
    Health health;
    Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        animator = sprite.GetComponent<Animator>();
        attacking = false;
        died = false;
        inPain = false;
        ResetAttackTime();
        StartCoroutine(AddPositionToList());
    }

    void Update()
    {
        // Follow target or stand still

        if (agent.enabled == true)
        {
            if (health.isDead == false && attacking == false && inPain == false && target != null && agent.path != null)
            {
                if (hasMeleeAttack == false)
                {
                    if (Vector3.Distance(transform.position, target.transform.position) > agent.stoppingDistance * 2f && goingToPreviousPosition == false)
                    {
                        agent.destination = target.transform.position;
                    }

                    if (Vector3.Distance(transform.position, target.transform.position) > agent.stoppingDistance * 2f && goingToPreviousPosition == true)
                    {
                        agent.destination = previousPositions[previousPositionSelected];
                    }

                    if (Vector3.Distance(transform.position, target.transform.position) >= agent.stoppingDistance * 4f && goingToPreviousPosition == true)
                    {
                        goingToPreviousPosition = false;
                    }

                    if (Vector3.Distance(transform.position, target.transform.position) <= agent.stoppingDistance * 2f && goingToPreviousPosition == true)
                    {
                        goingToPreviousPosition = false;
                    }

                    if (Vector3.Distance(transform.position, target.transform.position) <= agent.stoppingDistance * 2f && goingToPreviousPosition == false)
                    {
                        goingToPreviousPosition = true;
                    }
                }
                else
                {
                    agent.destination = target.transform.position;
                }
                animator.SetBool("Walking", true);
            }
            else
            {
                agent.destination = transform.position;
                animator.SetBool("Walking", false);
            }
        }

        if (target != null && health.isDead == false)
        {
            // Rotate towards target
            dir = (target.transform.position - transform.position).normalized;
            dir.y = 0;
            transform.forward = dir;


            // Reduce attack time
            if (CanSee(target, Mathf.Infinity))
            {
                if (hasRangedAttack == true)
                {
                    if (attacking == false && inPain == false)
                    {
                        attackTime -= Time.deltaTime;
                    }
                }
            }
        }

        // Attacks

        if (health.isDead == false && attacking == false && inPain == false && target != null)
        {
            if (attackTime <= 0 && hasRangedAttack == true)
            {
                StartCoroutine(Attack());
            }

            if (Vector3.Distance(transform.position, target.transform.position) < meleeRange && hasMeleeAttack == true)
            {
                StartCoroutine(AttackMelee());
            }
        }

        // Check death

        if(health.isDead == true && died == false)
        {
            died = true;
            StopAllCoroutines();
            Death();
        }
    }

    public IEnumerator Attack()
    {
        attacking = true;

        if (attackSound != null)
        {
            Instantiate(attackSound, transform.position, transform.rotation);
        }

        if (attackAnim != "")
        {
            animator.Play(attackAnim);
        }

        yield return new WaitForSeconds(attackShotDelay);

        if (attackProjectile != null)
        {
            var p = Instantiate(attackProjectile, transform.position + transform.forward, transform.rotation);
            p.GetComponent<Projectile>().ignoreTag = tag;
            p.transform.forward = dir;
        }

        yield return new WaitForSeconds(attackTotalDuration - attackShotDelay);

        ResetAttackTime();

        attacking = false;
    }

    public IEnumerator AttackMelee()
    {
        attacking = true;

        if (meleeSound != null)
        {
            Instantiate(meleeSound, transform.position, transform.rotation);
        }

        if (meleeAnim != "")
        {
            animator.Play(meleeAnim);
        }

        yield return new WaitForSeconds(meleeStartDelay);

        RaycastHit hitMelee;
        if (Physics.Raycast(transform.position, transform.forward, out hitMelee, meleeRange * 1.25f, sightMask))
        {
            Debug.DrawRay(transform.position, transform.forward * hitMelee.distance, Color.yellow, meleeRange);
            Debug.Log("Enemy Hit " + hitMelee.collider.name);

            if (hitMelee.collider.gameObject != null)
            {
                GameObject hitObject = hitMelee.collider.gameObject;

                if (hitObject.tag == "Player")
                {
                    hitObject.GetComponent<Health>().TakeDamage(meleeDamage, false);
                }
            }
        }

        yield return new WaitForSeconds(meleeDuration - meleeStartDelay);

        if (Vector3.Distance(transform.position, target.transform.position) >= meleeRange)
        {
            attacking = false;
        }
        else
        {
            StartCoroutine(AttackMelee());
        }
    }

    public void ResetAttackTime()
    {
        attackTime = attackTimeDefault * Random.Range(0.8f, 1.2f);
        attacking = false;
    }

    public IEnumerator Pain()
    {
        if (Random.Range(0, painChance) == 0)
        {
            inPain = true;

            StopCoroutine(Attack());
            ResetAttackTime();

            if (painAnim != "")
            {
                animator.Play(painAnim);
            }

            yield return new WaitForSeconds(painTime);

            inPain = false;
        }
        else
        {
            inPain = false;
        }
    }

    IEnumerator AddPositionToList()
    {
        yield return new WaitForSeconds(5f);

        if (health.isDead == false && target != null)
        {
            previousPositions[previousPositionsItem] = transform.position;
            previousPositionsItem++;
        }

        if(previousPositionsItem >= previousPositions.Length)
        {
            previousPositionsItem = 0;
        }

        if(Random.Range(0, 3) == 0)
        {
            previousPositionSelected = Random.Range(0, previousPositions.Length);
        }

        StartCoroutine(AddPositionToList());
    }

    public bool CanSee(GameObject which, float dist)
    {
        Vector3 dir;
        bool hasPath = false;

        dir = (which.transform.position - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir;

        if (agent.enabled == true)
        {
            NavMeshPath seePath = new NavMeshPath();
            hasPath = agent.CalculatePath(which.transform.position, seePath);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, dist, sightMask))
        {
            if (hit.collider.gameObject == which && hasPath)
            {
                Debug.Log(gameObject + " saw " + which);
                Debug.DrawRay(transform.position, dir * hit.distance, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(transform.position, dir * 1000, Color.red);
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public bool CanHear(GameObject which, float dist)
    {
        NavMeshPath hearPath = new NavMeshPath();

        if (agent.enabled == true)
        {
            agent.CalculatePath(which.transform.position, hearPath);
        }

        if (Vector3.Distance(transform.position, which.transform.position) < dist && hearPath.status == NavMeshPathStatus.PathComplete)
        {
            Debug.Log(gameObject + " can hear " + which);
            return true;
        }
        else
        {
            return false;
        }
    }

    void Death()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;
        Player.score += giveScoreOnDeath;
        animator.Play(deathAnim);
    }
}
