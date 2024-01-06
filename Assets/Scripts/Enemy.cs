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
    private float _projectileSpeedMult = 1.0f;
    private GameObject _attackSoundInstance;

    public enum RangedAttackType
    {
        OneObjectForwardAccurate,
        ThreeObjectsForwardSpread,
        OneRayForwardAccurate,
        OneRayForwardSpreadSmall,
        OneRayForwardSpreadMedium,
        OneRayForwardSpreadLarge,
        BossRay,
        BossProjectile
    }
    public RangedAttackType attackType = 0;

    public bool attackRefire = false;
    private Coroutine _attackCoroutine;

    // Melee attack
    [Header("Melee Attack")]
    public string meleeAnim;
    public int meleeDamage;
    public float meleeRange;
    public float meleeStartDelay;
    public float meleeDuration;
    public GameObject meleeSound;
    public GameObject meleeSoundHit;
    private Coroutine _meleeCoroutine;

    // Attack types
    [Header("Attack Types")]
    public bool hasRangedAttack = true;
    public bool hasMeleeAttack = false;

    // Sight
    [Header("Sight")]
    public LayerMask sightMask;
    public GameObject sightSound;
    public bool noSightSoundOnHard = false;

    // Drop item
    [Header("Item Drop")]
    public GameObject dropItem;

    // Misc health properties. The actual health and armor values are in the Health script.
    [Header("Misc health properties")]
    public bool healthNotAffectedByDifficulty = false;

    // Pain
    [Header("Pain")]
    public float painTime = 0.5f;
    public int painChance = 0;
    public string painAnim;
    public GameObject painSound;
    private bool _inPain = false;
    private GameObject _painSoundInstance;

    // Death
    [Header("Death")]
    public string deathAnim;
    public GameObject deathSound;
    private bool _attacking = false;
    private bool _died = false;

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
    private bool _wokeUp = false;

    // Healing
    [Header("Healing")]
    public bool canBeHealed = true;
    public bool canBeRevived = true;
    private bool _revived = false;

    // Previous position
    int previousPositionsItem = 0;
    int previousPositionSelected = 0;
    Vector3[] previousPositions = new Vector3[10];
    bool goingToPreviousPosition = false;

    // Related to saving and loading
    private string _initialPositionToString;

    private Vector3 _dir;
    private NavMeshAgent _agent;
    private Health _healthScript;
    private Animator _animator;
    private Player _player;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _healthScript = GetComponent<Health>();
        _animator = sprite.GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _attacking = false;
        _died = false;
        _inPain = false;
        _revived = false;

        if (StaticClass.difficulty < GetComponent<AppearOnDifficulty>().difficulty.Length && StaticClass.difficulty > -1)
        {
            // Don't add this enemy to counter if it's not meant to appear in this difficulty level.
            if (GetComponent<AppearOnDifficulty>().difficulty[StaticClass.difficulty] == true && StaticClass.gameState != 2)
            {
                StaticClass.enemiesTotal++;
                Debug.Log("Added " + gameObject.name + " to enemy count. (" + StaticClass.enemiesTotal + ")");
            }
        }
        else if (StaticClass.difficulty <= -1)
        {
            // No enemies.
            Destroy(gameObject);
            Debug.Log("Difficulty is " + StaticClass.difficulty.ToString() + ". Destroyed " + gameObject.name);
        }
        else if (StaticClass.gameState != 2)
        {
            // Keep this enemy if difficulty value is greater than the regular limit.
            StaticClass.enemiesTotal++;
            Debug.Log("Difficulty is " + StaticClass.difficulty.ToString() + ". This is greater than the regular limit. Kept " + gameObject.name);
        }

        if (painChance < 1)
        {
            painChance = 1;
        }

        if (StaticClass.difficulty <= 0)
        {
            _projectileSpeedMult = 0.75f;
            attackTimeDefault *= 1.1f;

            if (healthNotAffectedByDifficulty == false)
            {
                _healthScript.health = Mathf.RoundToInt(_healthScript.health * 0.9f);
            }

            // Make enemy ray attack less accurate if on Easy.
            if (attackType == RangedAttackType.OneRayForwardSpreadSmall)
            {
                attackType = RangedAttackType.OneRayForwardSpreadMedium;
            }
        }
        else if (StaticClass.difficulty == 1)
        {
            _projectileSpeedMult = 1.0f;
        }
        else if (StaticClass.difficulty == 2)
        {
            _projectileSpeedMult = 1.25f;
            _agent.speed *= 1.25f;
            attackTimeDefault *= 0.75f;

            if(healthNotAffectedByDifficulty == false)
            {
                _healthScript.health = Mathf.RoundToInt(_healthScript.health * 1.1f);
            }
        }
        else if (StaticClass.difficulty >= 3)
        {
            _projectileSpeedMult = 1.5f;
            _agent.speed *= 1.75f;
            attackTimeDefault *= 0.5f;
            painChance += 1;

            if (healthNotAffectedByDifficulty == false)
            {
                _healthScript.health = Mathf.RoundToInt(_healthScript.health * 1.25f);
            }

            // Make enemy ray attack more accurate if on Very Hard.
            if (attackType == RangedAttackType.OneRayForwardSpreadMedium || attackType == RangedAttackType.OneRayForwardSpreadLarge)
            {
                attackType = RangedAttackType.OneRayForwardSpreadSmall;
            }
        }

        // If this enemy has no sight sound on hard difficulty.
        if (noSightSoundOnHard && StaticClass.difficulty >= 2)
        {
            sightSound = null;
        }

        _initialPositionToString = transform.position.x.ToString() + transform.position.y.ToString() + transform.position.z.ToString();
        if (StaticClass.loadSavedMapData == false)
        {
            _player.enemyStartPositions.Add(_initialPositionToString);
        }

        ResetAttackTime();

        // Add positions for the retreat behaviour.
        StartCoroutine(AddPositionToListCoroutine());
        for (int i = 0; i < previousPositions.Length; i++)
        {
            AddPositionToList();
        }
    }

    void Update()
    {
        // Follow target or stand still.
        if (_agent.enabled == true)
        {
            if (_healthScript.isDead == false && _attacking == false && _inPain == false && wakeUpTimer <= 0f && target != null && _agent.path != null)
            {
                if (hasMeleeAttack == false)
                {
                    // If far away from target and not going back to previous position, go to the target's position.
                    if (Vector3.Distance(transform.position, target.transform.position) > _agent.stoppingDistance * 2f && goingToPreviousPosition == false)
                    {
                        _agent.destination = target.transform.position;
                    }

                    // Go back to previous locations.
                    if (Vector3.Distance(transform.position, target.transform.position) > _agent.stoppingDistance * 2f && goingToPreviousPosition == true)
                    {
                        _agent.destination = previousPositions[previousPositionSelected];
                    }

                    // If too far away from target, and has valid path, but still going back, stop.
                    if (Vector3.Distance(transform.position, target.transform.position) >= _agent.stoppingDistance * 4f && _agent.pathStatus == NavMeshPathStatus.PathComplete && goingToPreviousPosition == true)
                    {
                        goingToPreviousPosition = false;
                    }

                    // If this enemy is very close to the target, go back to previous locations.
                    if (Vector3.Distance(transform.position, target.transform.position) <= _agent.stoppingDistance * 2f && goingToPreviousPosition == false)
                    {
                        goingToPreviousPosition = true;
                    }

                    // If this enemy can see the target but path is partial, go back to previous locations.
                    if (_agent.pathStatus == NavMeshPathStatus.PathPartial && goingToPreviousPosition == false)
                    {
                        goingToPreviousPosition = true;
                        Debug.Log(gameObject + " can see the target but path is partial. Going back.");
                    }

                    // If this enemy can see the target but path is invalid.
                    if (_agent.pathStatus == NavMeshPathStatus.PathInvalid && goingToPreviousPosition == false)
                    {
                        goingToPreviousPosition = true;
                        Debug.Log(gameObject + " can see the target but path is invalid. Going back.");
                    }
                }
                else
                {
                    _agent.destination = target.transform.position;
                }
                _animator.SetBool("Walking", true);
            }
            else
            {
                _agent.destination = transform.position;
                _animator.SetBool("Walking", false);
            }
        }

        if (target != null && _healthScript.isDead == false)
        {
            // Rotate towards target
            _dir = (target.transform.position - transform.position).normalized;
            _dir.y = 0;
            transform.forward = _dir;


            // Reduce attack time
            if (CanSee(target, Mathf.Infinity))
            {
                if (hasRangedAttack == true)
                {
                    if (_attacking == false && _inPain == false && wakeUpTimer <= 0)
                    {
                        attackTime -= Time.deltaTime;

                        // If this enemy is very close to the player, attack faster.
                        if (Vector3.Distance(transform.position, target.transform.position) <= _agent.stoppingDistance * 2f)
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

            // Reduce wake up animation time.
            if (_wokeUp == false && _animator.enabled == true && sprite.activeSelf == true)
            {
                // Execute once when this enemy has a target.
                _wokeUp = true;
                _animator.Play(wakeUpAnim);

                // Save this interaction to a list in the player's script.
                _player.enemiesWithTargets.Add(_initialPositionToString);

                if (sightSound != null)
                {
                    Instantiate(sightSound, transform.position, transform.rotation);
                }
            }
            if (wakeUpTimer > 0f)
            {
                wakeUpTimer -= Time.deltaTime;
            }
            if (wakeUpTimer < 0f)
            {
                wakeUpTimer = 0f;
            }

            // Open doors
            RaycastHit doorHit;
            if (Physics.Raycast(transform.position, transform.forward, out doorHit, 4f))
            {
                if (doorHit.collider.gameObject.GetComponent<Door>() != null)
                {
                    Door doorScript = doorHit.collider.gameObject.GetComponent<Door>();
                    bool canOpen = true;

                    // If the enemy is targeting the player, it can only open the door if the player can open it too.
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
                        doorHit.collider.gameObject.GetComponent<Door>().StartCoroutine(doorScript.OpenDoor());
                    }
                }
            }
        }

        // Attacks
        if (_healthScript.isDead == false && _attacking == false && _inPain == false && target != null && wakeUpTimer <= 0f && StaticClass.gameState == 0)
        {
            if (attackTime <= 0 && hasRangedAttack == true)
            {
               _attackCoroutine = StartCoroutine(Attack());
            }

            if (Vector3.Distance(transform.position, target.transform.position) < meleeRange && hasMeleeAttack == true)
            {
                _meleeCoroutine = StartCoroutine(AttackMelee());
            }
        }

        // Check death
        if (_healthScript.isDead == true && _died == false && StaticClass.gameState == 0)
        {
            _died = true;
            StopAllCoroutines();
            Death(false);
        }
        _animator.SetBool("Dead", _healthScript.isDead);

        // Check if state changed
        // Disable this enemy if the player completed the current level or died.
        if (StaticClass.gameState == 1 || StaticClass.gameState == 2)
        {
            StopAllCoroutines();
            _agent.enabled = false;
            target = null;
        }

        // Hidden while waiting.
        if (hiddenWhileWaiting)
        {
            if (target == null && wakeUpTimer > 0f && _healthScript.isDead == false)
            {
                sprite.SetActive(false);
            }
            else
            {
                sprite.SetActive(true);
            }
        }

        // If enemy was killed on this save slot and its sprite is not active.
        if (_player.killedEnemies.Contains(_initialPositionToString) && sprite.activeSelf == false)
        {
            wakeUpTimer = 0;
            sprite.SetActive(true);
        }

        // If enemy was killed on this save slot.
        if (_player.killedEnemies.Contains(_initialPositionToString) && _healthScript.isDead == false)
        {
            _died = true;
            Death(true);
        }

        // If enemy spotted the player on this save slot.
        if (_player.enemiesWithTargets.Contains(_initialPositionToString) && _healthScript.isDead == false)
        {
            target = _player.gameObject;
        }
    }

    public IEnumerator Attack()
    {
        _attacking = true;

        if (attackSound != null)
        {
            _attackSoundInstance = Instantiate(attackSound, transform.position, transform.rotation);
        }

        if (attackAnim != "")
        {
            _animator.Play(attackAnim);
        }

        yield return new WaitForSeconds(attackShotDelay);

        if ((attackProjectile != null || attackDamage != 0) && _inPain == false)
        {
            // Create attack object(s) or ray(s).
            switch (attackType)
            {
                case RangedAttackType.OneObjectForwardAccurate:
                    ProjectileAttack(0, false, false);
                    break;

                case RangedAttackType.ThreeObjectsForwardSpread:
                    ProjectileAttack(0, false, false);
                    ProjectileAttack(0.3f, false, false);
                    ProjectileAttack(-0.3f, false, false);
                    break;

                case RangedAttackType.OneRayForwardAccurate:
                    RangedRayAttack(0f, false);
                    break;

                case RangedAttackType.OneRayForwardSpreadSmall:
                    RangedRayAttack(Random.Range(-0.03f, 0.03f), false);
                    break;

                case RangedAttackType.OneRayForwardSpreadMedium:
                    RangedRayAttack(Random.Range(-0.06f, 0.06f), false);
                    break;

                case RangedAttackType.OneRayForwardSpreadLarge:
                    RangedRayAttack(Random.Range(-0.12f, 0.12f), false);
                    break;

                case RangedAttackType.BossRay:
                    RangedRayAttack(0f, true);
                    attackRefire = true;

                    if (Random.Range(0, 4) == 0)
                    {
                        attackType = RangedAttackType.BossProjectile;
                    }
                    break;

                case RangedAttackType.BossProjectile:
                    ProjectileAttack(0f, true, true);
                    ProjectileAttack(1f, true, true);
                    ProjectileAttack(-1f, true, true);
                    attackRefire = false;
                    break;

                default:
                    break;
            }
        }

        yield return new WaitForSeconds(attackTotalDuration - attackShotDelay);

        if (attackRefire == false || CanSee(target, 50f) == false || _inPain == true)
        {
            ResetAttackTime();
            _attacking = false;

            if (attackType == RangedAttackType.BossProjectile)
            {
                attackTime = attackTimeDefault * 3f;
                attackType = RangedAttackType.BossRay;
            }
        }
        else
        {
            _attackCoroutine = StartCoroutine(Attack());
        }
    }

    public IEnumerator AttackMelee()
    {
        _attacking = true;

        if (meleeSound != null)
        {
            Instantiate(meleeSound, transform.position, transform.rotation);
        }

        if (meleeAnim != "")
        {
            _animator.Play(meleeAnim);
        }

        yield return new WaitForSeconds(meleeStartDelay);

        if (_inPain == false)
        {
            RaycastHit hitMelee;
            if (Physics.Raycast(transform.position, transform.forward, out hitMelee, meleeRange * 1.25f, sightMask))
            {
                if (StaticClass.debug == true)
                {
                    Debug.DrawRay(transform.position, transform.forward * hitMelee.distance, Color.yellow, 5f);
                    Debug.Log("Enemy Melee Hit " + hitMelee.collider.name);
                }

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
            _attacking = false;
        }
        else
        {
           _meleeCoroutine = StartCoroutine(AttackMelee());
        }
    }

    void RangedRayAttack(float maxSpread, bool ignoreInvisibility)
    {
        if (target.GetComponent<Player>() != null && ignoreInvisibility == false)
        {
            if (target.GetComponent<Player>().isInvisible == true)
            {
                maxSpread *= 5f;
            }
        }

        RaycastHit hitRanged;
        if (Physics.Raycast(transform.position, (transform.forward + (transform.right * maxSpread)), out hitRanged, 1000f, sightMask))
        {
            if (StaticClass.debug == true)
            {
                Debug.DrawRay(transform.position, (transform.forward + (transform.right * maxSpread)) * hitRanged.distance, Color.cyan, 20f);
                Debug.Log("Enemy Ranged Hit " + hitRanged.collider.name);
            }

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

    void ProjectileAttack(float spreadMult, bool ignoreDamageOverride, bool ignoreInvisibility)
    {
        var p = Instantiate(attackProjectile, transform.position + transform.forward, transform.rotation);
        p.GetComponent<Projectile>().ignoreTag = tag;
        p.GetComponent<Projectile>().speed *= _projectileSpeedMult;
        p.transform.forward = (transform.forward + transform.right * spreadMult).normalized;

        if (attackDamage != 0 && !ignoreDamageOverride)
        {
            p.GetComponent<Projectile>().damage = attackDamage;
        }

        if (target.GetComponent<Player>() != null)
        {
            if (target.GetComponent<Player>().isInvisible && !ignoreInvisibility)
            {
                if (spreadMult == 0.0f)
                {
                    p.transform.forward = (transform.forward + transform.right / Random.Range(-4f, 4f)).normalized;
                }
                else if (spreadMult > 0.0f)
                {
                    p.transform.forward = (transform.forward + transform.right / Random.Range(4f, 6f)).normalized;
                }
                else if (spreadMult < 0.0f)
                {
                    p.transform.forward = (transform.forward + -transform.right / Random.Range(4f, 6f)).normalized;
                }
            }
        }
    }

    public void ResetAttackTime()
    {
        attackTime = attackTimeDefault * Random.Range(0.8f, 1.2f);
        _attacking = false;
    }

    public IEnumerator Pain()
    {
        if (Random.Range(0, painChance) == 0 && _inPain == false)
        {
            _inPain = true;
            StopAttackSound();

            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
            }
            if (_meleeCoroutine != null)
            {
                StopCoroutine(_meleeCoroutine);
            }

            if (StaticClass.difficulty <= 0)
            {
                ResetAttackTime();
            }
            if (painAnim != "")
            {
                _animator.Play(painAnim);
            }
            if (painSound != null && _healthScript.isDead == false)
            {
                _painSoundInstance = Instantiate(painSound, transform.position, transform.rotation);
            }

            yield return new WaitForSeconds(painTime);

            _attacking = false;
            _inPain = false;
        }
    }

    IEnumerator AddPositionToListCoroutine()
    {
        AddPositionToList();

        yield return new WaitForSeconds(1f);

        StartCoroutine(AddPositionToListCoroutine());
    }

    void AddPositionToList()
    {
        if (_healthScript.isDead == false)
        {
            previousPositions[previousPositionsItem] = new Vector3(transform.position.x + Random.Range(-8, 9), transform.position.y, transform.position.z + Random.Range(-8, 9));
            previousPositionsItem++;
        }

        if (previousPositionsItem >= previousPositions.Length)
        {
            previousPositionsItem = 0;
        }

        previousPositionSelected = Random.Range(0, previousPositions.Length);
    }

    public bool CanSee(GameObject which, float dist)
    {
        Vector3 dir;
        bool hasPath = false;

        dir = (which.transform.position - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir;

        if (_agent.enabled == true)
        {
            NavMeshPath seePath = new NavMeshPath();
            hasPath = _agent.CalculatePath(which.transform.position, seePath);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, dist, sightMask))
        {
            var hitObj = hit.collider.gameObject;

            if (hitObj == which && (hitObj.GetComponent<Player>() == null || hitObj.GetComponent<Player>().isInvisible == false) || hitObj == target)
            {
                if (StaticClass.debug == true)
                {
                    Debug.Log(gameObject + " saw " + which);
                    Debug.DrawRay(transform.position, dir * hit.distance, Color.green);
                }
                return true;
            }
            else
            {
                if (StaticClass.debug == true)
                {
                    Debug.DrawRay(transform.position, dir * 500, Color.red);
                }
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

        if (_agent.enabled == true)
        {
            _agent.CalculatePath(which.transform.position, hearPath);
        }

        if (Vector3.Distance(transform.position, which.transform.position) < dist && hearPath.status == NavMeshPathStatus.PathComplete)
        {
            if (StaticClass.debug == true)
            {
                Debug.Log(gameObject + " can hear " + which);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Teleporter")
        {
            Teleporter teleScript = other.gameObject.GetComponent<Teleporter>();

            if (teleScript.enemyCanUse)
            {
                teleScript.Teleport(gameObject);
            }
        }
    }

    void StopAttackSound()
    {
        if (_attackSoundInstance != null)
        {
            Destroy(_attackSoundInstance);
        }
    }

    public void Revive()
    {
        _healthScript.health = _healthScript.startHealth;
        _healthScript.isDead = false;
        _attacking = false;
        _died = false;
        _inPain = false;
        _revived = true;
        StopAttackSound();
        ResetAttackTime();

        tag = "Enemy";
        gameObject.layer = 9;
        _animator.Play(painAnim);

        GetComponent<Collider>().isTrigger = false;
        GetComponent<Collider>().enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

        _player.killedEnemies.Remove(_initialPositionToString);
    }

    void Death(bool instant)
    {
        // Disable components.
        GetComponent<Collider>().isTrigger = true;
        GetComponent<NavMeshAgent>().enabled = false;

        _healthScript.health = 0;
        _player.killedEnemies.Add(_initialPositionToString);
        wakeUpTimer = 0;
        _wokeUp = true;

        // Give score.
        if (instant == false)
        {
            Player.scoreThisLevel += giveScoreOnDeath;
        }

        // Only count towards kill count once.
        if (_revived == false)
        {
            StaticClass.enemiesKilled++;
        }

        // Set tag to ProjectileIgnores and set layer to IgnoreRaycast, play death animation.
        tag = "ProjectileIgnores";
        gameObject.layer = 2;

        if (_animator.enabled == true)
        {
            if (instant == false)
            {
                _animator.Play(deathAnim);
            }
            else
            {
                _animator.Play(deathAnim + "Stay");
            }
        }

        if (canBeRevived == false)
        {
            Destroy(GetComponent<Collider>());
        }

        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position - (transform.up * 2) + (transform.forward / 2), transform.rotation);
        }

        StopAttackSound();

        // Prevent pain sound from playing.
        if (_painSoundInstance != null)
        {
            Destroy(_painSoundInstance);
        }

        // Play death sound, if there is one.
        if (deathSound != null && instant == false)
        {
            Instantiate(deathSound, transform.position, transform.rotation);
        }
    }
}
