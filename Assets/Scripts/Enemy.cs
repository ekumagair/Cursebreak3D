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
    [Header("Ranged Attack")]
    public float attackTimeDefault;
    public float attackTime = 0;
    public GameObject attackProjectile;
    [Tooltip("Necessary for raycast attacks. Optional for projectiles. (If not 0, overrides the projectile damage)")]
    public int attackDamage;
    public string attackAnim;
    public float attackShotDelay = 0;
    public float attackTotalDuration;
    public GameObject attackSound;
    public GameObject[] attackExtraObject;
    float projectileSpeedMult = 1.0f;

    public enum RangedAttackType
    {
        OneObjectForwardAccurate,
        ThreeObjectsForwardSpread,
        OneRayForwardAccurate,
        OneRayForwardSpread
    }
    public RangedAttackType attackType = 0;

    // Melee attack
    [Header("Melee Attack")]
    public string meleeAnim;
    public int meleeDamage;
    public float meleeRange;
    public float meleeStartDelay;
    public float meleeDuration;
    public GameObject meleeSound;
    public GameObject meleeSoundHit;

    // Attack types
    [Header("Attack Types")]
    public bool hasRangedAttack = true;
    public bool hasMeleeAttack = false;

    // Death
    [Header("Death")]
    public string deathAnim;
    public GameObject deathSound;
    bool attacking = false;
    bool died = false;

    // Sight
    [Header("Sight")]
    public LayerMask sightMask;

    // Drop item
    [Header("Item Drop")]
    public GameObject dropItem;

    // Pain
    bool inPain = false;
    [Header("Pain")]
    public float painTime = 0.5f;
    public int painChance = 0;
    public string painAnim;

    // Pain chance
    // 1 = Always
    // 2 = 50% chance
    // 3 = 33% chance
    // 4 = 25% chance
    // 5+ = Smaller chance

    // Wake Up Animation
    [Header("Wake Up Animation")]
    public bool hiddenWhileWaiting = false;
    public float wakeUpTimer = 0f;
    public string wakeUpAnim = "";
    bool wokeUp = false;

    // Previous position
    int previousPositionsItem = 0;
    int previousPositionSelected = 0;
    Vector3[] previousPositions = new Vector3[10];
    bool goingToPreviousPosition = false;

    Vector3 dir;
    NavMeshAgent agent;
    Health healthScript;
    Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        healthScript = GetComponent<Health>();
        animator = sprite.GetComponent<Animator>();
        attacking = false;
        died = false;
        inPain = false;

        StaticClass.enemiesTotal++;

        if(painChance < 1)
        {
            painChance = 1;
        }

        if (StaticClass.difficulty <= 0)
        {
            projectileSpeedMult = 0.9f;
        }
        else if (StaticClass.difficulty == 1)
        {
            projectileSpeedMult = 1.0f;
        }
        else if (StaticClass.difficulty == 2)
        {
            projectileSpeedMult = 1.1f;
            healthScript.health = Mathf.RoundToInt(healthScript.health * 1.1f);
            agent.speed *= 1.1f;
            attackTimeDefault *= 0.9f;
        }
        else if (StaticClass.difficulty >= 3)
        {
            projectileSpeedMult = 1.25f;
            healthScript.health = Mathf.RoundToInt(healthScript.health * 1.25f);
            agent.speed *= 1.75f;
            attackTimeDefault *= 0.5f;
            painChance += 1;

            // Make enemy ray attack more accurate if on Very Hard.
            if(attackType == RangedAttackType.OneRayForwardSpread)
            {
                attackType = RangedAttackType.OneRayForwardAccurate;
            }
        }

        ResetAttackTime();
        StartCoroutine(AddPositionToList());
    }

    void Update()
    {
        // Follow target or stand still.
        if (agent.enabled == true)
        {
            if (healthScript.isDead == false && attacking == false && inPain == false && wakeUpTimer <= 0f && target != null && agent.path != null)
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

                    // If this enemy is very close to the player, go back to previous locations.
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

        if (target != null && healthScript.isDead == false)
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

                        // If this enemy is very close to the player, attack faster.
                        if (Vector3.Distance(transform.position, target.transform.position) <= agent.stoppingDistance * 2f)
                        {
                            attackTime -= Time.deltaTime;

                            // If you are playing on Very Hard difficulty, this makes the attack even faster.
                            if(StaticClass.difficulty >= 3)
                            {
                                attackTime -= Time.deltaTime * 2;
                            }
                        }
                    }
                }
            }

            // Reduce wake up animation time
            if(wokeUp == false && animator.enabled == true && sprite.activeSelf == true)
            {
                wokeUp = true;
                animator.Play(wakeUpAnim);
            }
            if (wakeUpTimer > 0f)
            {
                wakeUpTimer -= Time.deltaTime;
            }
            if(wakeUpTimer < 0f)
            {
                wakeUpTimer = 0f;
            }

            // Open doors
            RaycastHit doorHit;
            if (Physics.Raycast(transform.position, transform.forward, out doorHit, 4f, sightMask))
            {
                if (doorHit.collider.gameObject.GetComponent<Door>() != null)
                {
                    Door doorScript = doorHit.collider.gameObject.GetComponent<Door>();
                    bool canOpen = true;

                    // If the enemy is targeting the player, it can only open the door if the player can too.
                    if (target.GetComponent<Player>() != null)
                    {
                        if (target.GetComponent<Player>().keys[doorScript.key])
                        {
                            canOpen = true;
                        }
                        else
                        {
                            canOpen = false;
                        }
                    }

                    if (doorScript.doorState == 0 && doorScript.canUse == true && canOpen == true)
                    {
                        StartCoroutine(doorScript.OpenDoor());
                    }
                }
            }
        }

        // Attacks
        if (healthScript.isDead == false && attacking == false && inPain == false && target != null && wakeUpTimer <= 0f && StaticClass.gameState == 0)
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
        if(healthScript.isDead == true && died == false && StaticClass.gameState == 0)
        {
            died = true;
            StopAllCoroutines();
            Death();
        }

        // Check if state changed
        // Disable this enemy if the player completed the current level or died
        if(StaticClass.gameState == 1 || StaticClass.gameState == 2)
        {
            StopAllCoroutines();
            agent.enabled = false;
            target = null;
        }

        // Hidden while waiting
        if(hiddenWhileWaiting)
        {
            if(target == null && wakeUpTimer > 0f)
            {
                sprite.SetActive(false);
            }
            else
            {
                sprite.SetActive(true);
            }
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

        if ((attackProjectile != null || attackDamage != 0) && inPain == false)
        {
            if (attackType == RangedAttackType.OneObjectForwardAccurate)
            {
                var p = Instantiate(attackProjectile, transform.position + transform.forward, transform.rotation);
                p.GetComponent<Projectile>().ignoreTag = tag;
                p.GetComponent<Projectile>().speed *= projectileSpeedMult;
                p.transform.forward = dir;

                if (attackDamage != 0)
                {
                    p.GetComponent<Projectile>().damage = attackDamage;
                }

                if (target.GetComponent<Player>() != null)
                {
                    if(target.GetComponent<Player>().isInvisible)
                    {
                        p.transform.forward = (transform.forward + transform.right / Random.Range(-4f, 4f)).normalized;
                    }
                }
            }
            else if (attackType == RangedAttackType.ThreeObjectsForwardSpread)
            {
                var p1 = Instantiate(attackProjectile, transform.position + transform.forward, transform.rotation);
                p1.GetComponent<Projectile>().ignoreTag = tag;
                p1.GetComponent<Projectile>().speed *= projectileSpeedMult;
                p1.transform.forward = dir;

                var p2 = Instantiate(attackProjectile, transform.position + transform.forward, transform.rotation);
                p2.GetComponent<Projectile>().ignoreTag = tag;
                p2.GetComponent<Projectile>().speed *= projectileSpeedMult;
                p2.transform.forward = (transform.forward + transform.right / 3).normalized;

                var p3 = Instantiate(attackProjectile, transform.position + transform.forward, transform.rotation);
                p3.GetComponent<Projectile>().ignoreTag = tag;
                p3.GetComponent<Projectile>().speed *= projectileSpeedMult;
                p3.transform.forward = (transform.forward + -transform.right / 3).normalized;

                if(attackDamage != 0)
                {
                    p1.GetComponent<Projectile>().damage = attackDamage;
                    p2.GetComponent<Projectile>().damage = attackDamage;
                    p3.GetComponent<Projectile>().damage = attackDamage;
                }

                if (target.GetComponent<Player>() != null)
                {
                    if (target.GetComponent<Player>().isInvisible)
                    {
                        p1.transform.forward = (transform.forward + transform.right / Random.Range(-4f, 4f)).normalized;
                        p2.transform.forward = (transform.forward + transform.right / Random.Range(4f, 6f)).normalized;
                        p2.transform.forward = (transform.forward + -transform.right / Random.Range(4f, 6f)).normalized;
                    }
                }
            }
            else if(attackType == RangedAttackType.OneRayForwardAccurate)
            {
                RangedRayAttack(0f, false);
            }
            else if (attackType == RangedAttackType.OneRayForwardSpread)
            {
                RangedRayAttack(Random.Range(-0.03f, 0.03f), false);
            }
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

        if (inPain == false)
        {
            RaycastHit hitMelee;
            if (Physics.Raycast(transform.position, transform.forward, out hitMelee, meleeRange * 1.25f, sightMask))
            {
                Debug.DrawRay(transform.position, transform.forward * hitMelee.distance, Color.yellow, 5f);
                Debug.Log("Enemy Melee Hit " + hitMelee.collider.name);

                if (hitMelee.collider.gameObject != null)
                {
                    GameObject hitObject = hitMelee.collider.gameObject;

                    if (hitObject.tag == "Player")
                    {
                        hitObject.GetComponent<Health>().TakeDamage(meleeDamage, false);
                    }

                    if (meleeSoundHit != null)
                    {
                        Instantiate(meleeSoundHit, transform.position, transform.rotation);
                    }
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

    void RangedRayAttack(float maxSpread, bool ignoreInvisibility)
    {
        if(target.GetComponent<Player>() != null && ignoreInvisibility == false)
        {
            if (target.GetComponent<Player>().isInvisible == true)
            {
                maxSpread *= 4f;
            }
        }

        RaycastHit hitRanged;
        if (Physics.Raycast(transform.position, (transform.forward + (transform.right * maxSpread)), out hitRanged, 1000f, sightMask))
        {
            Debug.DrawRay(transform.position, (transform.forward + (transform.right * maxSpread)) * hitRanged.distance, Color.cyan, 5f);
            Debug.Log("Enemy Ranged Hit " + hitRanged.collider.name);

            if (hitRanged.collider.gameObject != null)
            {
                GameObject hitObject = hitRanged.collider.gameObject;

                if (hitObject.tag == "Player")
                {
                    hitObject.GetComponent<Health>().TakeDamage(attackDamage, false);
                }

                if (attackExtraObject[0] != null)
                {
                    Instantiate(attackExtraObject[0], transform.position, transform.rotation);
                }
                if (attackExtraObject[1] != null)
                {
                    Instantiate(attackExtraObject[1], hitRanged.point - transform.forward, transform.rotation);
                }
                if (attackExtraObject[2] != null)
                {
                    Instantiate(attackExtraObject[2], hitRanged.point - transform.forward, transform.rotation);
                }
            }
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
            StopCoroutine(AttackMelee());

            if (StaticClass.difficulty <= 0)
            {
                ResetAttackTime();
            }

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
        yield return new WaitForSeconds(2f);

        if (healthScript.isDead == false)
        {
            previousPositions[previousPositionsItem] = new Vector3(transform.position.x * Random.Range(0.75f, 1.25f), transform.position.y, transform.position.z * Random.Range(0.75f, 1.25f));
            previousPositionsItem++;
        }

        if(previousPositionsItem >= previousPositions.Length)
        {
            previousPositionsItem = 0;
        }

        if(Random.Range(0, 1) == 0)
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
            var hitObj = hit.collider.gameObject;

            if (hitObj == which && hasPath && (hitObj.GetComponent<Player>() == null || hitObj.GetComponent<Player>().isInvisible == false) || hitObj == target)
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
        Player.scoreThisLevel += giveScoreOnDeath;
        StaticClass.enemiesKilled++;
        animator.Play(deathAnim);

        if(dropItem != null)
        {
            Instantiate(dropItem, transform.position - (transform.up * 2) + (transform.forward / 2), transform.rotation);
        }

        if(deathSound != null)
        {
            Instantiate(deathSound, transform.position, transform.rotation);
        }
    }
}
