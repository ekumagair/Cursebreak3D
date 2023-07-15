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
    public GameObject extraDamageSound;
    public bool isInvisible = false;
    public RenderTexture canvasTargetTexture;

    // score: Total chapter score. Only resets after a chapter is completed. Is only actually updated after a level ends.
    public static int score = 0;
    // scoreThisLevel: Resets after level is completed.
    public static int scoreThisLevel = 0;

    // Time spent on the current level.
    public static int timeSeconds = 0;
    public static int timeMinutes = 0;

    // Player information stored after a level is completed.
    public static int savedHealth = 100;
    public static int savedArmor = 100;
    public static float savedArmorMult = 1f;
    public static bool[] savedWeaponsUnlocked = new bool[7];
    public static int[] savedAmmo = new int[3];

    public static Vector3 savedPosition;
    public static float savedRotation;
    public static int savedCurrentWeapon;
    public static float[] savedConditionTimers = new float[7];

    // Replace ammo display with scoreThisLevel display for a limited time.
    public static float gotScoreTimer = 0;

    // Damage prevents player from sprinting for a short time.
    public static bool damageStopsSprint = false;

    bool scrolledMouse = false;

    Health healthScript;
    Controls controlsScript;
    GameObject gameCanvas;
    HUD gameCanvasScript;
    Minimap minimapScript;
    CharacterController characterController;
    MapProperties mapProperties;

    // Register interaction with objects in the map.
    public List<string> enemyStartPositions;
    public static string[] savedEnemyStartPositions = new string[1];
    public List<string> killedEnemies;
    public static string[] savedKilledEnemies = new string[1];
    public List<string> destroyedItemsPositions;
    public static string[] savedDestroyedItemsPositions = new string[1];
    public List<string> discoveredSecrets;
    public static string[] savedDiscoveredSecrets = new string[1];
    public List<string> enemiesWithTargets;
    public static string[] savedEnemiesWithTargets = new string[1];

    // Weapon list
    // 0 = Empty hand (Always unlocked)
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
    // 5 = Can't aim horizontally
    // 6 = Can't aim vertically

    private void Awake()
    {
        StaticClass.enemiesKilled = 0;
        StaticClass.secretsDiscovered = 0;
        StaticClass.enemiesTotal = 0;
        StaticClass.secretsTotal = 0;
    }

    void Start()
    {
        healthScript = GetComponent<Health>();
        controlsScript = GetComponent<Controls>();
        gameCanvas = GameObject.FindGameObjectWithTag("Canvas");
        gameCanvasScript = gameCanvas.GetComponent<HUD>();
        minimapScript = gameCanvasScript.mapRoot.GetComponent<Minimap>();
        characterController = GetComponent<CharacterController>();
        mapProperties = GameObject.Find("MapProperties").GetComponent<MapProperties>();
        Camera.main.targetTexture = canvasTargetTexture;
        isInvisible = false;
        weaponsUnlocked[0] = true;
        scrolledMouse = false;
        StaticClass.gameState = 0;
        TitleScreen.startFromChapterSelect = false;

        if (StaticClass.loadSavedPlayerInfo == true)
        {
            // Load player info.
            healthScript.health = savedHealth;
            healthScript.armor = savedArmor;
            healthScript.armorMult = savedArmorMult;
            currentWeapon = savedCurrentWeapon;

            for (int i = 0; i < savedAmmo.Length; i++)
            {
                ammo[i] = savedAmmo[i];
            }

            for (int i = 0; i < savedWeaponsUnlocked.Length; i++)
            {
                weaponsUnlocked[i] = savedWeaponsUnlocked[i];
            }

            // Fail-safe, in case the player's saved health is 0 or less.
            if (healthScript.health <= 0)
            {
                healthScript.health = 1;
            }

            Debug.Log("Loaded player inventory info.");

            // Load full player info.
            if (StaticClass.loadSavedPlayerFullInfo == true)
            {
                characterController.enabled = false;
                transform.position = new Vector3(savedPosition.x, savedPosition.y, savedPosition.z);
                transform.rotation = Quaternion.Euler(0, savedRotation, 0);

                for (int i = 0; i < savedConditionTimers.Length; i++)
                {
                    conditionTimer[i] = savedConditionTimers[i];
                }

                conditionTimer[1] = 0.5f;
                conditionTimer[5] = 0.5f;
                conditionTimer[6] = 0.5f;

                Debug.Log("Loaded player full info.");
            }
        }
        else
        {
            // Don't load player info.
            StaticClass.secretsDiscovered = 0;
            StaticClass.enemiesKilled = 0;
            timeSeconds = 0;
            timeMinutes = 0;
            scoreThisLevel = 0;
        }

        // Load saved map data.
        if (StaticClass.loadSavedMapData == true)
        {
            for (int x = 0; x < savedEnemyStartPositions.Length; x++)
            {
                enemyStartPositions.Add(savedEnemyStartPositions[x]);
            }
            for (int i = 0; i < savedDestroyedItemsPositions.Length; i++)
            {
                destroyedItemsPositions.Add(savedDestroyedItemsPositions[i]);
            }
            for (int y = 0; y < savedKilledEnemies.Length; y++)
            {
                killedEnemies.Add(savedKilledEnemies[y]);
            }
            for (int i = 0; i < savedDiscoveredSecrets.Length; i++)
            {
                discoveredSecrets.Add(savedDiscoveredSecrets[i]);
            }
            for (int i = 0; i < savedEnemiesWithTargets.Length; i++)
            {
                enemiesWithTargets.Add(savedEnemiesWithTargets[i]);
            }
        }

        // Reduce damage vulnerability on easier difficulties. Increase damage vulnerability on harder difficulties.
        if (StaticClass.difficulty <= 0) // Easy
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
        else if (StaticClass.difficulty >= 3) // Very hard
        {
            healthScript.overallDamageMult = 2.0f;
        }

        // If difficulty is normal or easier.
        if(StaticClass.difficulty <= 1)
        {
            damageStopsSprint = false;
        }
        else
        {
            damageStopsSprint = true;
        }

        StartCoroutine(CheckEnemyVision());
        StartCoroutine(Timer());

        if (StaticClass.pendingLoad > -1)
        {
            // If loaded slot from a menu that doesn't have the player.
            StaticClass.ResetStats(false);
            SaveSystem.LoadGame(StaticClass.pendingLoad);
        }
        else
        {
            StartCoroutine(Autosave(0.25f));
        }

        characterController.enabled = true;
    }

    void Update()
    {
        // Select weapon with keyboard.
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
            weaponDelaysCurrent[6] = weaponDelaysDefault[6];
        }

        // Select weapon with mouse wheel.
        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f && HUD.minimapEnabled == false)
        {
            if (scrolledMouse == false)
            {
                currentWeapon = NextAvailableWeapon(-Input.GetAxisRaw("Mouse ScrollWheel"));
                scrolledMouse = true;
            }
        }
        else
        {
            scrolledMouse = false;
        }

        // Attack
        if (Input.GetMouseButton(0) && weaponsUnlocked[currentWeapon] == true && weaponDelaysCurrent[currentWeapon] == 0 && (ammo[weaponAmmoType[currentWeapon]] >= weaponAmmoCost[currentWeapon] || weaponAmmoType[currentWeapon] == -1) && healthScript.isDead == false && StaticClass.gameState == 0)
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
                    GameObject.Find("ElectricFlash").GetComponent<Animator>().Play("ElectricFlash");
                    rayDamage = 30;
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

            // Play weapon sound.
            if (weaponSound[currentWeapon] != null)
            {
                Instantiate(weaponSound[currentWeapon], transform.position, transform.rotation);
            }

            // If the player has an extra damage power-up, play an additional sound.
            if(extraDamageSound != null && (conditionTimer[3] > 0 || conditionTimer[4] > 0))
            {
                Instantiate(extraDamageSound, transform.position, transform.rotation);
            }

            // Consume ammo and make the weapon take some time to fire again.
            ammo[weaponAmmoType[currentWeapon]] -= weaponAmmoCost[currentWeapon];
            weaponDelaysCurrent[currentWeapon] = weaponDelaysDefault[currentWeapon];

            // Change to melee if you ran out of ammo.
            if (ammo[weaponAmmoType[currentWeapon]] < weaponAmmoCost[currentWeapon])
            {
                if (weaponsUnlocked[1] == false)
                {
                    currentWeapon = 0;
                }
                else
                {
                    currentWeapon = 1;
                    weaponDelaysCurrent[1] = weaponDelaysDefault[1];
                }
            }
        }

        // Weapon delays
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
        }
        else
        {
            isInvisible = false;
        }

        // Condition timers
        // Cycle through every condition timer.
        for (int i = 0; i < conditionTimer.Length; i++)
        {
            if (conditionTimer[i] > 0)
            {
                conditionTimer[i] -= Time.deltaTime;

                // Show condition overlay.
                if (gameCanvasScript.conditionOverlays[i] != null)
                {
                    gameCanvasScript.conditionOverlays[i].enabled = true;

                    if (gameCanvasScript.conditionOverlaysAnimators[i] != null)
                    {
                        if (conditionTimer[i] > 3)
                        {
                            // Show overlay.
                            gameCanvasScript.conditionOverlaysAnimators[i].Play("FlashImageShow");
                        }
                        else
                        {
                            // Make overlay blink to show that the condition timer is running out. Don't blink if power-up flashing is disabled.
                            if (Options.flashingEffects == 0 || Options.flashingEffects == 3)
                            {
                                gameCanvasScript.conditionOverlaysAnimators[i].Play("FlashImage");
                            }
                            else
                            {
                                gameCanvasScript.conditionOverlaysAnimators[i].Play("FlashImageShow");
                            }
                        }
                    }
                }

                // Invisibility runs out faster if sprinting.
                if (i == 0 && controlsScript.isSprinting == true)
                {
                    conditionTimer[i] -= Time.deltaTime * 0.5f;
                }
            }
            if (conditionTimer[i] < 0)
            {
                conditionTimer[i] = 0;
            }
            if (conditionTimer[i] <= 0)
            {
                // Hide condition overlay.
                if (gameCanvasScript.conditionOverlays[i] != null)
                {
                    gameCanvasScript.conditionOverlays[i].enabled = false;
                }
            }
        }

        // Pause
        if(Input.GetKeyDown(KeyCode.Escape) && !Input.GetKey(KeyCode.Alpha1) && StaticClass.canPause && StaticClass.gameState != 1 && StaticClass.gameState != 2)
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

        // Reveal walls on minimap
        if (StaticClass.minimapType == 2 && HUD.minimapEnabled == false)
        {
            for (float i = -1; i <= 1; i += 0.1f)
            {
                RevealMinimapRay(i, false);
            }
        }

        // Reveal floor on minimap
        //minimapScript.AddFloorToMinimap(gameObject);

        // Debug
        if (StaticClass.debug == true)
        {
            // Toggle canvas
            if(Input.GetKeyDown(KeyCode.U))
            {
                gameCanvas.SetActive(!gameCanvas.activeSelf);

                if (gameCanvas.activeSelf)
                {
                    Camera.main.targetTexture = canvasTargetTexture;
                }
                else
                {
                    Camera.main.targetTexture = null;
                }
            }

            // Add object in front of player to minimap.
            if(Input.GetKeyDown(KeyCode.M))
            {
                RevealMinimapRay(0, true);
            }

            // Add health
            if(Input.GetKeyDown(KeyCode.H) && !Input.GetKey(KeyCode.LeftShift))
            {
                healthScript.health += 10;
            }
            // Add armor
            if (Input.GetKeyDown(KeyCode.R) && !Input.GetKey(KeyCode.LeftShift))
            {
                healthScript.armorMult = 0.75f;
                healthScript.armor += 10;
            }
            if (Input.GetKeyDown(KeyCode.G) && !Input.GetKey(KeyCode.LeftShift))
            {
                healthScript.armorMult = 0.5f;
                healthScript.armor += 10;
            }

            // Save and Load tests
            if(Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    SaveToPlayerData(0);
                }
                if (Input.GetKeyDown(KeyCode.H))
                {
                    LoadFromPlayerData(0);
                }
                if (Input.GetKeyDown(KeyCode.B))
                {
                    SaveSystem.SaveGame(0);
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    SaveSystem.LoadGame(0);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Player Y rotation: " + transform.rotation.y);
                Debug.Log("Difficulty is " + StaticClass.difficulty);
                Debug.Log("Level score: " + Player.scoreThisLevel);
                Debug.Log("Chapter score: " + Player.score);
                Debug.Log("Slot 1 level score: " + PlayerPrefs.GetInt("slot1_scoreThisLevel"));
            }
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
                // Give health.
                if (itemScript.giveHealth != 0)
                {
                    healthScript.health += itemScript.giveHealth;

                    // Don't autosave if the map properties forbid it.
                    if(mapProperties.healthItemDoesNotAutosave == true)
                    {
                        itemScript.triggersAutosave = false;
                    }
                }

                // Limits player health.
                if (healthScript.health > 100)
                {
                    healthScript.health = 100;
                }

                // Give weapon.
                if (itemScript.giveWeapon >= 0)
                {
                    weaponsUnlocked[itemScript.giveWeapon] = true;

                    // Select new weapon if your hand is empty.
                    if (currentWeapon == 0 && CanSelectWeapon(itemScript.giveWeapon))
                    {
                        currentWeapon = itemScript.giveWeapon;
                    }
                }

                // Give ammo.
                if (itemScript.giveAmmo > 0 && itemScript.giveAmmoType != -1)
                {
                    ammo[itemScript.giveAmmoType] += itemScript.giveAmmo;
                }

                // Give armor.
                if (itemScript.giveArmor != 0)
                {
                    healthScript.armor += itemScript.giveArmor;

                    // Replace current armor with better armor.
                    if(itemScript.giveArmorMult < healthScript.armorMult)
                    {
                        healthScript.armorMult = itemScript.giveArmorMult;
                    }

                    // Don't autosave if the map properties forbid it.
                    if (mapProperties.armorItemDoesNotAutosave == true)
                    {
                        itemScript.triggersAutosave = false;
                    }
                }

                // Give score.
                if (itemScript.giveScore != 0)
                {
                    scoreThisLevel += itemScript.giveScore;
                    gotScoreTimer = 4f;
                }

                // Give key.
                if (itemScript.giveKey >= 0)
                {
                    keys[itemScript.giveKey] = true;
                }

                // Give condition.
                if (itemScript.giveCondition > -1 && itemScript.giveConditionTimer > 0)
                {
                    conditionTimer[itemScript.giveCondition] = itemScript.giveConditionTimer;
                }

                // Message on top left corner.
                if (itemScript.logMessageOnCollect != "")
                {
                    gameCanvas.GetComponent<HUD>().HudAddLog(itemScript.logMessageOnCollect);
                }

                // Pickup flash.
                if (itemScript.createOnCollect != null && (Options.flashingEffects == 0 || Options.flashingEffects == 1))
                {
                    Instantiate(itemScript.createOnCollect, gameCanvas.transform);
                }

                if (itemScript.createOnCollectGameWorld != null)
                {
                    Instantiate(itemScript.createOnCollectGameWorld, transform.position, transform.rotation);
                }

                if (itemScript.triggersAutosave == true && itemScript.giveHealth >= 0)
                {
                    StartCoroutine(Autosave(0.5f));
                }

                destroyedItemsPositions.Add(other.gameObject.transform.position.x.ToString() + other.gameObject.transform.position.y.ToString() + other.gameObject.transform.position.z.ToString());
                Destroy(other.gameObject);
            }
        }
        else if (otherObject.tag == "Teleporter")
        {
            Teleporter teleScript = otherObject.GetComponent<Teleporter>();

            if (teleScript.playerCanUse)
            {
                teleScript.Teleport(gameObject);
            }
        }
    }

    public IEnumerator CheckEnemyVision()
    {
        yield return new WaitForSeconds(0.05f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            foreach (GameObject en in enemies)
            {
                if (en.GetComponent<Enemy>() != null)
                {
                    Enemy enemyScript = en.GetComponent<Enemy>();
                    if (enemyScript.CanSee(gameObject, 600f))
                    {
                        enemyScript.target = gameObject;
                    }
                }
            }
        }

        StartCoroutine(CheckEnemyVision());
    }

    // Time spent on this level.
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

    // Auto save on slot 0.
    public IEnumerator Autosave(float t)
    {
        yield return new WaitForSeconds(t);

        // Can only autosave if the player is alive.
        if (StaticClass.gameState != 2)
        {
            Debug.Log("Autosaved on slot 0.");
            SaveSystem.SaveGame(0);
        }
        else
        {
            Debug.Log("Tried to autosave on slot 0, but the player is dead. Autosave cancelled.");
        }
    }

    // When the player takes damage.
    public void PlayerPain(int amount)
    {
        if(StaticClass.difficulty > 0 && amount > 9 && damageStopsSprint == true)
        {
            // Don't let player sprint for a limited time if more than 9 damage was taken.
            conditionTimer[2] = 1f;
        }

        // Damage flash.
        if(damageOverlayObject != null && (Options.flashingEffects == 0 || Options.flashingEffects == 2))
        {
            Instantiate(damageOverlayObject, gameCanvas.transform);
        }
    }

    // When the player dies.
    IEnumerator PlayerDeath()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Controls>().enabled = false;
        Camera.main.GetComponent<MouseLook>().enabled = false;
        HUD.minimapEnabled = false;

        Instantiate(deathFadeObject, gameCanvas.transform);

        StaticClass.gameState = 2;
        StaticClass.enemiesTotal = 0;
        StaticClass.secretsTotal = 0;

        yield return new WaitForSeconds(4f);

        SaveSystem.LoadGame(0);
        SceneManager.LoadScene("C" + StaticClass.currentChapter + "M" + StaticClass.currentMap);
    }

    // Exit level.
    public IEnumerator Exit(GameObject fade)
    {
        StaticClass.gameState = 1;
        StaticClass.pendingLoad = -1;
        Time.timeScale = 1.0f;

        GetComponent<CharacterController>().enabled = false;
        GetComponent<Controls>().enabled = false;
        Camera.main.GetComponent<MouseLook>().enabled = false;

        if (fade != null)
        {
            Instantiate(fade, gameCanvas.transform);
        }

        // Add score from this level to chapter total.
        score += scoreThisLevel;

        // Save info.
        savedHealth = healthScript.health;
        savedArmor = healthScript.armor;
        savedArmorMult = healthScript.armorMult;
        savedCurrentWeapon = currentWeapon;

        for (int i = 0; i < ammo.Length; i++)
        {
            savedAmmo[i] = ammo[i];
        }

        for (int i = 0; i < weaponsUnlocked.Length; i++)
        {
            savedWeaponsUnlocked[i] = weaponsUnlocked[i];
        }

        // Don't load old data on the new level.
        StaticClass.loadSavedPlayerInfo = true;
        StaticClass.loadSavedPlayerFullInfo = false;
        StaticClass.loadSavedMapData = false;

        yield return new WaitForSeconds(1.85f);

        SceneManager.LoadScene("Intermission");
    }

    // Pause
    public void PauseStart()
    {
        Cursor.lockState = CursorLockMode.None;
        StaticClass.gameState = 3;
        Time.timeScale = 0.0f;

        gameCanvas.GetComponent<HUD>().GoToPauseRoot();
    }

    // Unpause
    public void PauseEnd()
    {
        Cursor.lockState = CursorLockMode.Locked;
        StaticClass.gameState = 0;
        Time.timeScale = 1.0f;
    }

    // Check if a weapon can be selected.
    bool CanSelectWeapon(int w)
    {
        return currentWeapon != w && weaponsUnlocked[w] && ammo[weaponAmmoType[w]] >= weaponAmmoCost[w];
    }

    // Check what weapon can be selected next while using the mouse wheel. Skips weapons you can't choose.
    int NextAvailableWeapon(float direction)
    {
        int select = currentWeapon;

        if (direction > 0)
        {
            for (int i = 0; i < weaponsUnlocked.Length; i++)
            {
                if (i > select && CanSelectWeapon(i))
                {
                    select = i;
                    break;
                }
            }
        }
        else if (direction < 0)
        {
            for (int i = weaponsUnlocked.Length; i > 0; i--)
            {
                if (i < select && CanSelectWeapon(i))
                {
                    select = i;
                    break;
                }
            }
        }

        return select;
    }

    // Multiply damage for the player's attacks, depending on certain conditions.
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

    // Reveal minimap with sight.
    void RevealMinimapRay(float spread, bool addToLog)
    {
        RaycastHit minimapHit;
        if (Physics.Raycast(transform.position, transform.forward + (spread * transform.right), out minimapHit, Mathf.Infinity, attackRayMask))
        {
            if (minimapHit.collider.gameObject != null)
            {
                minimapScript.AddToMinimapFilter(minimapHit.collider.gameObject);

                if (addToLog == true && StaticClass.debug == true)
                {
                    gameCanvas.GetComponent<HUD>().HudAddLog(minimapHit.collider.gameObject.ToString());
                    gameCanvas.GetComponent<HUD>().HudAddLog(minimapHit.collider.gameObject.transform.position.ToString());
                }
            }
        }
    }

    // Save to player data.
    public void SaveToPlayerData(int slot)
    {
        SaveSystem.SavePlayer(this, slot);
    }

    // Load from player data.
    public void LoadFromPlayerData(int slot)
    {
        PlayerData data = SaveSystem.LoadPlayer(slot);

        characterController.enabled = false;

        savedHealth = data.health;
        savedArmor = data.armor;
        savedArmorMult = data.armorMult;
        savedCurrentWeapon = data.currentWeapon;

        savedPosition.x = data.position[0];
        savedPosition.y = data.position[1];
        savedPosition.z = data.position[2];

        savedRotation = data.rotation;

        StaticClass.difficulty = data.difficulty;
        Player.score = data.score;
        Player.scoreThisLevel = data.scoreThisLevel;
        Player.timeSeconds = data.timeSeconds;
        Player.timeMinutes = data.timeMinutes;

        for (int i = 0; i < ammo.Length; i++)
        {
            savedAmmo[i] = data.ammo[i];
        }

        for (int i = 0; i < weaponsUnlocked.Length; i++)
        {
            savedWeaponsUnlocked[i] = data.weaponsUnlocked[i];
        }

        savedEnemyStartPositions = new string[data.enemyStartPositions.Length];
        for (int i = 0; i < data.enemyStartPositions.Length; i++)
        {
            savedEnemyStartPositions[i] = data.enemyStartPositions[i];
        }

        savedDestroyedItemsPositions = new string[data.destroyedItemsPositions.Length];
        for (int i = 0; i < data.destroyedItemsPositions.Length; i++)
        {
            savedDestroyedItemsPositions[i] = data.destroyedItemsPositions[i];
        }

        savedKilledEnemies = new string[data.killedEnemies.Length];
        for (int i = 0; i < data.killedEnemies.Length; i++)
        {
            savedKilledEnemies[i] = data.killedEnemies[i];
        }

        savedDiscoveredSecrets = new string[data.discoveredSecrets.Length];
        for (int i = 0; i < data.discoveredSecrets.Length; i++)
        {
            savedDiscoveredSecrets[i] = data.discoveredSecrets[i];
        }

        savedConditionTimers = new float[data.condition.Length];
        for (int i = 0; i < data.condition.Length; i++)
        {
            savedConditionTimers[i] = data.condition[i];
        }

        savedEnemiesWithTargets = new string[data.enemiesWithTarget.Length];
        for (int i = 0; i < data.enemiesWithTarget.Length; i++)
        {
            savedEnemiesWithTargets[i] = data.enemiesWithTarget[i];
        }

        StaticClass.loadSavedPlayerInfo = true;
        StaticClass.loadSavedPlayerFullInfo = true;
        StaticClass.loadSavedMapData = true;

        SceneManager.LoadScene(data.scene);
    }
}
