using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int currentWeapon = 0;
    public bool[] weaponsUnlocked;
    public int[] weaponType;
    public float[] weaponRayRange;
    public float[] weaponDelaysDefault;
    public float[] weaponDelaysCurrent;
    public GameObject[] weaponProjectile;
    public GameObject[] weaponSound;
    public GameObject[] weaponSound2;
    public int[] weaponAmmoType;
    public int[] weaponAmmoCost;
    public int[] ammo;
    public int[] ammoLimit;
    public bool[] keys;
    public LayerMask attackRayMask;
    public GameObject deathFadeObject;

    public static int score = 0;
    public static int timeSeconds = 0;
    public static int timeMinutes = 0;
    public static int savedHealth = 100;
    public static int savedArmor = 100;
    public static float savedArmorMult = 1f;
    public static bool[] savedWeaponsUnlocked = new bool[7];
    public static int[] savedAmmo = new int[2];

    Health healthScript;
    GameObject gameCanvas;

    // Weapon list
    // 0 = Empty hand
    // 1 = Wooden Staff
    // 2 = Plasma Blade
    // 3 = Fire Ring

    // Key list
    // 0 = Can always open
    // 1 = Bronze key
    // 2 = Silver key
    // 3 = Gold key

    void Start()
    {
        healthScript = GetComponent<Health>();
        gameCanvas = GameObject.FindGameObjectWithTag("Canvas");
        weaponsUnlocked[0] = true;
        timeSeconds = 0;
        timeMinutes = 0;
        StaticClass.gameState = 0;
        StaticClass.secretsDiscovered = 0;

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

            StaticClass.loadSavedPlayerInfo = false;
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
        if(Input.GetKeyDown(KeyCode.Alpha1) && weaponsUnlocked[1])
        {
            currentWeapon = 1;
            weaponDelaysCurrent[4] = weaponDelaysDefault[4];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponsUnlocked[2])
        {
            currentWeapon = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponsUnlocked[3])
        {
            currentWeapon = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && weaponsUnlocked[4])
        {
            currentWeapon = 4;
            weaponDelaysCurrent[1] = weaponDelaysDefault[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha5) && weaponsUnlocked[5])
        {
            currentWeapon = 5;
        }
        //Debug.Log("Current weapon: " + currentWeapon);

        // Attack

        if (Input.GetMouseButton(0) && weaponDelaysCurrent[currentWeapon] == 0 && (ammo[weaponAmmoType[currentWeapon]] >= weaponAmmoCost[currentWeapon] || weaponAmmoType[currentWeapon] == -1) && healthScript.isDead == false && StaticClass.gameState == 0)
        {
            if (weaponType[currentWeapon] == 0 && weaponProjectile[currentWeapon] != null)
            {
                var p = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                p.GetComponent<Projectile>().ignoreTag = tag;

                if(currentWeapon == 3)
                {
                    var p2 = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                    p2.GetComponent<Projectile>().ignoreTag = tag;
                    p2.transform.forward = (transform.forward + transform.right / 5).normalized;

                    var p3 = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                    p3.GetComponent<Projectile>().ignoreTag = tag;
                    p3.transform.forward = (transform.forward + -transform.right / 5).normalized;
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
                    rayDamage = 40;
                }

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
                    }
                }
            }

            if(weaponType[currentWeapon] != -1)
            {
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject en in enemies)
                {
                    if (en.GetComponent<Enemy>() != null)
                    {
                        if (en.GetComponent<Enemy>().CanHear(gameObject, 600f))
                        {
                            en.GetComponent<Enemy>().target = gameObject;
                        }
                    }
                }
            }

            if (weaponSound[currentWeapon] != null)
            {
                Instantiate(weaponSound[currentWeapon], transform.position, transform.rotation);
            }

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

        // Change to melee if ran out of ammo

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

        // Change to staff if your hand is empty

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

                    if(itemScript.giveArmorMult < healthScript.armorMult)
                    {
                        healthScript.armorMult = itemScript.giveArmorMult;
                    }
                }

                if (itemScript.giveScore != 0)
                {
                    score += itemScript.giveScore;
                }

                if (itemScript.giveKey >= 0)
                {
                    keys[itemScript.giveKey] = true;
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
        yield return new WaitForSeconds(0.2f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            foreach (GameObject en in enemies)
            {
                if (en.GetComponent<Enemy>() != null)
                {
                    Enemy enemyScript = en.GetComponent<Enemy>();
                    if (enemyScript.CanSee(gameObject, 200f))
                    {
                        enemyScript.target = gameObject;
                    }
                }
            }
        }

        StartCoroutine(CheckEnemyVision());
    }

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

    IEnumerator PlayerDeath()
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Controls>().enabled = false;
        Camera.main.GetComponent<MouseLook>().enabled = false;
        Instantiate(deathFadeObject, gameCanvas.transform);
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("C" + StaticClass.chapterReadOnly + "M" + StaticClass.mapReadOnly);
    }

    public IEnumerator Exit(GameObject fade)
    {
        StaticClass.gameState = 1;
        GetComponent<CharacterController>().enabled = false;
        GetComponent<Controls>().enabled = false;
        Camera.main.GetComponent<MouseLook>().enabled = false;
        Instantiate(fade, gameCanvas.transform);

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

        yield return new WaitForSeconds(1.2f);

        //SceneManager.LoadScene("C" + StaticClass.chapterReadOnly + "M" + (StaticClass.mapReadOnly + 1));
        SceneManager.LoadScene("Intermission");
    }
}
