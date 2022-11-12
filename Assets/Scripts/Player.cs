using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int currentWeapon = 0;
    public bool[] weaponsUnlocked;
    public int[] weaponType;
    public float[] weaponRayRange;
    public float[] weaponDelaysDefault;
    public float[] weaponDelaysCurrent;
    public GameObject[] weaponProjectile;
    public GameObject[] weaponRayObjOnHit;
    public GameObject[] weaponSound;
    public GameObject[] weaponSound2;
    public int[] weaponAmmoType;
    public int[] weaponAmmoCost;
    public int[] ammo;
    public int[] ammoLimit;
    public bool[] keys;
    public float[] conditionTimer;
    public LayerMask attackRayMask;
    public GameObject deathFadeObject;
    public GameObject damageOverlayObject;
    public bool isInvisible = false;

    public static int score = 0;
    public static int scoreThisLevel = 0;
    public static int timeSeconds = 0;
    public static int timeMinutes = 0;
    public static int savedHealth = 100;
    public static int savedArmor = 100;
    public static float savedArmorMult = 1f;
    public static bool[] savedWeaponsUnlocked = new bool[7];
    public static int[] savedAmmo = new int[3];
    public static float gotScoreTimer = 0;

    Health healthScript;
    Controls controlsScript;
    GameObject gameCanvas;
    Image invisOverlay;
    Animator invisOverlayAnimator;
    Image doubleDamageOverlay;
    Animator doubleDamageOverlayAnimator;

    // Weapon list
    // 0 = Empty hand
    // 1 = Wooden Staff
    // 2 = Plasma Blade
    // 3 = Fire Ring
    // 4 = Sword
    // 5 = Rapid Fire Plasma
    // 6 = Electricity Staff

    // Weapon type list
    // -1 = Can't attack
    // 0 = Shoot ray
    // 1 = Shoot projectile

    // Ammo types
    // 0 = Plasma
    // 1 = Fire
    // 2 = Electricity

    // Key list
    // 0 = None, can always open
    // 1 = Bronze key
    // 2 = Silver key
    // 3 = Gold key

    // Condition List
    // 0 = Invisible
    // 1 = Can't walk
    // 2 = Can't sprint
    // 3 = 2x Damage Multiplier
    // 4 = 4x Damage Multiplier

    void Start()
    {
        healthScript = GetComponent<Health>();
        controlsScript = GetComponent<Controls>();
        gameCanvas = GameObject.FindGameObjectWithTag("Canvas");
        invisOverlay = gameCanvas.GetComponent<HUD>().invisOverlay;
        invisOverlayAnimator = gameCanvas.GetComponent<HUD>().invisOverlay.GetComponent<Animator>();
        doubleDamageOverlay = gameCanvas.GetComponent<HUD>().doubleDamageOverlay;
        doubleDamageOverlayAnimator = gameCanvas.GetComponent<HUD>().doubleDamageOverlay.GetComponent<Animator>();
        isInvisible = false;
        weaponsUnlocked[0] = true;
        timeSeconds = 0;
        timeMinutes = 0;
        StaticClass.gameState = 0;
        StaticClass.secretsDiscovered = 0;
        StaticClass.enemiesKilled = 0;

        if(StaticClass.loadSavedPlayerInfo == true)
        {
            healthScript.health = savedHealth;
            healthScript.armor = savedArmor;
            healthScript.armorMult = savedArmorMult;

            for (int i = 0; i < savedAmmo.Length; i++)
            {
                ammo[i] = savedAmmo[i];
            }

            for (int i = 0; i < savedWeaponsUnlocked.Length; i++)
            {
                weaponsUnlocked[i] = savedWeaponsUnlocked[i];
            }

            //StaticClass.loadSavedPlayerInfo = false;
            StartCoroutine(Autosave(0.03f));
        }

        if(StaticClass.difficulty == 0) // Easy
        {
            healthScript.overallDamageMult = 0.5f;
        }
        else if (StaticClass.difficulty == 1) // Normal
        {
            healthScript.overallDamageMult = 1.0f;
        }
        else if (StaticClass.difficulty == 2) // Hard
        {
            healthScript.overallDamageMult = 1.5f;
        }
        else if (StaticClass.difficulty == 3) // Very hard
        {
            healthScript.overallDamageMult = 2.0f;
        }

        StartCoroutine(CheckEnemyVision());
        StartCoroutine(Timer());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && CanSelectWeapon(1))
        {
            currentWeapon = 1;
            weaponDelaysCurrent[4] = weaponDelaysDefault[4];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && CanSelectWeapon(2))
        {
            currentWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && CanSelectWeapon(3))
        {
            currentWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && CanSelectWeapon(4))
        {
            currentWeapon = 4;
            weaponDelaysCurrent[1] = weaponDelaysDefault[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && CanSelectWeapon(5))
        {
            currentWeapon = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6) && CanSelectWeapon(6))
        {
            currentWeapon = 6;
        }

        // Attack
        if (Input.GetMouseButton(0) && weaponDelaysCurrent[currentWeapon] == 0 && (ammo[weaponAmmoType[currentWeapon]] >= weaponAmmoCost[currentWeapon] || weaponAmmoType[currentWeapon] == -1) && healthScript.isDead == false && StaticClass.gameState == 0)
        {
            if (weaponType[currentWeapon] == 0 && weaponProjectile[currentWeapon] != null)
            {
                var p = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                p.GetComponent<Projectile>().ignoreTag = tag;
                p.GetComponent<Projectile>().damage *= DamageMultiplier();

                if(currentWeapon == 3)
                {
                    var p2 = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                    p2.GetComponent<Projectile>().ignoreTag = tag;
                    p2.transform.forward = (transform.forward + transform.right / 8).normalized;
                    p2.GetComponent<Projectile>().damage *= DamageMultiplier();

                    var p3 = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                    p3.GetComponent<Projectile>().ignoreTag = tag;
                    p3.transform.forward = (transform.forward + -transform.right / 8).normalized;
                    p3.GetComponent<Projectile>().damage *= DamageMultiplier();
                }
            }
            else if (weaponType[currentWeapon] == 1)
            {
                int rayDamage = 0;

                if (currentWeapon == 1)
                {
                    GameObject.Find("WoodenStaff").GetComponent<Animator>().Play("WoodenStaffSwing");
                    rayDamage = 20;
                }
                else if (currentWeapon == 4)
                {
                    GameObject.Find("Sword").GetComponent<Animator>().Play("SwordSwing");
                    rayDamage = 45;
                }
                else if (currentWeapon == 6)
                {
                    rayDamage = 35;
                }

                rayDamage *= DamageMultiplier();

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, weaponRayRange[currentWeapon], attackRayMask))
                {
                    Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green, weaponRayRange[currentWeapon]);
                    Debug.Log("Player Hit " + hit.collider.name);

                    if (hit.collider.gameObject != null)
                    {
                        GameObject hitObject = hit.collider.gameObject;

                        if(hitObject.tag == "Enemy")
                        {
                            hitObject.GetComponent<Health>().TakeDamage(rayDamage, false);
                        }

                        if (weaponSound2[currentWeapon] != null)
                        {
                            Instantiate(weaponSound2[currentWeapon], transform.position, transform.rotation);
                        }
                        if (weaponRayObjOnHit[currentWeapon] != null)
                        {
                            Instantiate(weaponRayObjOnHit[currentWeapon], hit.point - transform.forward, transform.rotation);
                        }
                    }
                }
            }

            // Enemies can hear the attack.
            if(weaponType[currentWeapon] != -1)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject en in enemies)
                {
                    if (en.GetComponent<Enemy>() != null)
                    {
                        if (en.GetComponent<Enemy>().CanHear(gameObject, 800f))
                        {
                            en.GetComponent<Enemy>().target = gameObject;
                        }
                    }
                }
            }

            // Make sound.
            if (weaponSound[currentWeapon] != null)
            {
                Instantiate(weaponSound[currentWeapon], transform.position, transform.rotation);
            }

            // Consume ammo and make the weapon take some time to fire again.
            ammo[weaponAmmoType[currentWeapon]] -= weaponAmmoCost[currentWeapon];
            weaponDelaysCurrent[currentWeapon] = weaponDelaysDefault[currentWeapon];
        }

        for (int i = 0; i < weaponDelaysDefault.Length; i++)
        {
            if(weaponDelaysCurrent[i] > 0 && currentWeapon == i)
            {
                weaponDelaysCurrent[i] -= Time.deltaTime;
            }
            if(weaponDelaysCurrent[i] < 0)
            {
                weaponDelaysCurrent[i] = 0;
            }
        }

        // Change to melee if you ran out of ammo.
        if(weaponsUnlocked[currentWeapon] == false || ammo[weaponAmmoType[currentWeapon]] < weaponAmmoCost[currentWeapon])
        {
            if(weaponsUnlocked[1] == false)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon = 1;
                weaponDelaysCurrent[1] = weaponDelaysDefault[1];
            }
        }

        // Change to staff if your hand is empty.
        if(weaponsUnlocked[1] == true && currentWeapon == 0)
        {
            currentWeapon = 1;
        }

        // Ammo limits
        for (int i = 0; i < ammoLimit.Length; i++)
        {
            if(ammo[i] > ammoLimit[i])
            {
                ammo[i] = ammoLimit[i];
            }
        }

        // Armor limits
        if(healthScript.armor < 0)
        {
            healthScript.armor = 0;
        }
        if(healthScript.armor == 0)
        {
            healthScript.armorMult = 1;
        }
        if (healthScript.armor > 100)
        {
            healthScript.armor = 100;
        }

        // Death
        if(healthScript.isDead == true && StaticClass.gameState == 0)
        {
            StaticClass.gameState = 2;
            StartCoroutine(PlayerDeath());
        }

        // Conditions
        if (conditionTimer[0] > 0)
        {
            isInvisible = true;
            invisOverlay.enabled = true;
            if (conditionTimer[0] > 3)
            {
                invisOverlayAnimator.Play("FlashImageShow");
            }
            else
            {
                invisOverlayAnimator.Play("FlashImage");
            }
        }
        else
        {
            isInvisible = false;
            invisOverlay.enabled = false;
        }

        if(conditionTimer[3] > 0)
        {
            doubleDamageOverlay.enabled = true;
            if (conditionTimer[3] > 3)
            {
                doubleDamageOverlayAnimator.Play("FlashImageShow");
            }
            else
            {
                doubleDamageOverlayAnimator.Play("FlashImage");
            }
        }
        else
        {
            doubleDamageOverlay.enabled = false;
        }

        // Condition timers
        for (int i = 0; i < conditionTimer.Length; i++)
        {
            if (conditionTimer[i] > 0)
            {
                conditionTimer[i] -= Time.deltaTime;

                // Invisibility runs out faster if sprinting.
                if(i == 0 && controlsScript.isSprinting == true)
                {
                    conditionTimer[i] -= Time.deltaTime * 2f;
                }
            }
            if (conditionTimer[i] < 0)
            {
                conditionTimer[i] = 0;
            }
        }

        // Pause
        if(Input.GetKeyDown(KeyCode.Escape) && StaticClass.canPause && StaticClass.gameState != 1 && StaticClass.gameState != 2)
        {
            if(Time.timeScale == 0.0f)
            {
                PauseEnd();
            }
            else
            {
                PauseStart();
            }
        }

        // Show score
        if(gotScoreTimer > 0)
        {
            gotScoreTimer -= Time.deltaTime;
        }
        if (gotScoreTimer < 0)
        {
            gotScoreTimer = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;

        if(otherObject.tag == "Item")
        {
            Item itemScript = otherObject.GetComponent<Item>();

            if ((itemScript.giveHealth > 0 && healthScript.health < 100) || (itemScript.giveArmor > 0 && healthScript.armor < 100) || (itemScript.giveAmmo > 0 && ammo[itemScript.giveAmmoType] < ammoLimit[itemScript.giveAmmoType]) || itemScript.giveWeapon != -1 || itemScript.giveKey != -1 || itemScript.canAlwaysCollect == true)
            {
                if (itemScript.giveHealth != 0)
                {
                    healthScript.health += itemScript.giveHealth;
                }

                if (healthScript.health > 100)
                {
                    healthScript.health = 100;
                }

                if (itemScript.giveWeapon >= 0)
                {
                    weaponsUnlocked[itemScript.giveWeapon] = true;

                    // Select new weapon if your hand is empty.
                    if (currentWeapon == 0)
                    {
                        currentWeapon = itemScript.giveWeapon;
                    }
                }

                if (itemScript.giveAmmo > 0 && itemScript.giveAmmoType != -1)
                {
                    ammo[itemScript.giveAmmoType] += itemScript.giveAmmo;
                }

                if (itemScript.giveArmor != 0)
                {
                    healthScript.armor += itemScript.giveArmor;

                    // Replace current armor with better armor.
                    if(itemScript.giveArmorMult < healthScript.armorMult)
                    {
                        healthScript.armorMult = itemScript.giveArmorMult;
                    }
                }

                if (itemScript.giveScore != 0)
                {
                    score += itemScript.giveScore;
                    scoreThisLevel += itemScript.giveScore;
                    gotScoreTimer = 4f;
                }

                if (itemScript.giveKey >= 0)
                {
                    keys[itemScript.giveKey] = true;
                }

                if(itemScript.giveCondition > -1)
                {
                    conditionTimer[itemScript.giveCondition] = itemScript.giveConditionTimer;
                }

                if (itemScript.logMessageOnCollect != "")
                {
                    gameCanvas.GetComponent<HUD>().HudAddLog(itemScript.logMessageOnCollect);
                }

                if (itemScript.createOnCollect != null)
                {
                    Instantiate(itemScript.createOnCollect, gameCanvas.transform);
                }

                if (itemScript.createOnCollectGameWorld != null)
                {
                    Instantiate(itemScript.createOnCollectGameWorld, transform.position, transform.rotation);
                }

                Destroy(other.gameObject);
            }
        }
    }

    public IEnumerator CheckEnemyVision()
    {
        yield return new WaitForSeconds(0.1f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            foreach (GameObject en in enemies)
            {
                if (en.GetComponent<Enemy>() != null)
                {
                    Enemy enemyScript = en.GetComponent<Enemy>();
                    if (enemyScript.CanSee(gameObject, 500f))
                    {
                        enemyScript.target = gameObject;
                    }
                }
            }
        }

        StartCoroutine(CheckEnemyVision());
    }

    // Time spent on this level
    public IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);

        timeSeconds++;

        if(timeSeconds >= 60)
        {
            timeSeconds = 0;
            timeMinutes++;
        }

        StartCoroutine(Timer());
    }

    // Auto save on slot 0
    public IEnumerator Autosave(float t)
    {
        yield return new WaitForSeconds(t);
        gameCanvas.GetComponent<HUD>().SaveData(0);
    }

    // When the player takes damage
    public void PlayerPain()
    {
        conditionTimer[2] = 1f;

        if(damageOverlayObject != null)
        {
            Instantiate(damageOverlayObject, gameCanvas.transform);
        }
    }

    // When the player dies
    IEnumerator PlayerDeath()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Controls>().enabled = false;
        Camera.main.GetComponent<MouseLook>().enabled = false;
        HUD.mapEnabled = false;

        Instantiate(deathFadeObject, gameCanvas.transform);

        StaticClass.gameState = 2;
        StaticClass.enemiesTotal = 0;
        StaticClass.secretsTotal = 0;

        yield return new WaitForSeconds(4f);

        SceneManager.LoadScene("C" + StaticClass.currentChapter + "M" + StaticClass.currentMap);
    }

    public IEnumerator Exit(GameObject fade)
    {
        StaticClass.gameState = 1;
        Time.timeScale = 1.0f;

        GetComponent<CharacterController>().enabled = false;
        GetComponent<Controls>().enabled = false;
        Camera.main.GetComponent<MouseLook>().enabled = false;

        if (fade != null)
        {
            Instantiate(fade, gameCanvas.transform);
        }

        savedHealth = healthScript.health;
        savedArmor = healthScript.armor;
        savedArmorMult = healthScript.armorMult;

        for (int i = 0; i < ammo.Length; i++)
        {
            savedAmmo[i] = ammo[i];
        }

        for (int i = 0; i < weaponsUnlocked.Length; i++)
        {
            savedWeaponsUnlocked[i] = weaponsUnlocked[i];
        }

        StaticClass.loadSavedPlayerInfo = true;

        yield return new WaitForSeconds(1.3f);

        //SceneManager.LoadScene("C" + StaticClass.chapterReadOnly + "M" + (StaticClass.mapReadOnly + 1));
        SceneManager.LoadScene("Intermission");
    }

    public void PauseStart()
    {
        Cursor.lockState = CursorLockMode.None;
        StaticClass.gameState = 3;
        Time.timeScale = 0.0f;

        gameCanvas.GetComponent<HUD>().GoToPauseRoot();
    }

    public void PauseEnd()
    {
        Cursor.lockState = CursorLockMode.Locked;
        StaticClass.gameState = 0;
        Time.timeScale = 1.0f;
    }

    bool CanSelectWeapon(int w)
    {
        return weaponsUnlocked[w] && ammo[weaponAmmoType[w]] >= weaponAmmoCost[w];
    }

    int DamageMultiplier()
    {
        int mult = 1;

        if (conditionTimer[3] > 0)
        {
            mult *= 2;
        }
        if (conditionTimer[4] > 0)
        {
            mult *= 4;
        }

        return mult;
    }
}
