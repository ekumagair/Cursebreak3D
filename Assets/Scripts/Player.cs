using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int currentWeapon = 0;
    public bool[] weaponsUnlocked;
    public int[] weaponType;
    public float[] weaponRayRange;
    public float[] weaponDelaysDefault;
    public float[] weaponDelaysCurrent;
    public GameObject[] weaponProjectile;
    public int[] weaponAmmoType;
    public int[] weaponAmmoCost;
    public int[] ammo;
    public LayerMask attackRayMask;

    public GameObject soundSwoosh;

    Health healthScript;
    GameObject gameCanvas;

    // Weapon list
    // 0 = Empty hand
    // 1 = Wooden Staff
    // 2 = Magic Blade
    // 3 = Fire Ring

    void Start()
    {
        healthScript = GetComponent<Health>();
        gameCanvas = GameObject.FindGameObjectWithTag("Canvas");
        weaponsUnlocked[0] = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) && weaponsUnlocked[1])
        {
            currentWeapon = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponsUnlocked[2])
        {
            currentWeapon = 2;
        }
        //Debug.Log("Current weapon: " + currentWeapon);

        // Attack

        if (Input.GetMouseButton(0) && weaponDelaysCurrent[currentWeapon] == 0 && (ammo[weaponAmmoType[currentWeapon]] >= weaponAmmoCost[currentWeapon] || weaponAmmoType[currentWeapon] == -1) && healthScript.isDead == false)
        {
            if (weaponType[currentWeapon] == 0 && weaponProjectile[currentWeapon] != null)
            {
                var p = Instantiate(weaponProjectile[currentWeapon], transform.position + transform.forward, transform.rotation);
                p.GetComponent<Projectile>().ignoreTag = tag;
            }
            else if (weaponType[currentWeapon] == 1)
            {
                int rayDamage = 0;

                if (currentWeapon == 1)
                {
                    GameObject.Find("WoodenStaff").GetComponent<Animator>().Play("WoodenStaffSwing");
                    rayDamage = 20;
                    Instantiate(soundSwoosh, transform.position, transform.rotation);
                }

                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, weaponRayRange[currentWeapon], attackRayMask))
                {
                    Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green, weaponRayRange[currentWeapon]);
                    Debug.Log("Hit " + hit.collider.name);

                    if (hit.collider.gameObject != null)
                    {
                        GameObject hitObject = hit.collider.gameObject;

                        if(hitObject.tag == "Enemy")
                        {
                            if (currentWeapon == 1)
                            {
                                hitObject.GetComponent<Health>().TakeDamage(rayDamage, false);
                            }
                        }
                    }
                }
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

        if(weaponsUnlocked[currentWeapon] == false || ammo[weaponAmmoType[currentWeapon]] < weaponAmmoCost[currentWeapon])
        {
            if(weaponsUnlocked[1] == false)
            {
                currentWeapon = 0;
            }
            else
            {
                currentWeapon = 1;
            }
        }

        if(weaponsUnlocked[1] == true && currentWeapon == 0)
        {
            currentWeapon = 1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;

        if(otherObject.tag == "Item")
        {
            Item itemScript = otherObject.GetComponent<Item>();

            if(itemScript.giveHealth != 0)
            {
                healthScript.health += itemScript.giveHealth;
            }

            if(itemScript.giveWeapon >= 0)
            {
                weaponsUnlocked[itemScript.giveWeapon] = true;
                
                if(currentWeapon == 0)
                {
                    currentWeapon = itemScript.giveWeapon;
                }
            }

            if(itemScript.giveAmmo > 0 && itemScript.giveAmmoType != -1)
            {
                ammo[itemScript.giveAmmoType] += itemScript.giveAmmo;
            }

            if(itemScript.createOnCollect != null)
            {
                Instantiate(itemScript.createOnCollect, gameCanvas.transform);
            }

            Destroy(other.gameObject);
        }
    }
}
