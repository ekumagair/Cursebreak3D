using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 100;
    public bool isDead = false;
    public int armor = 0;
    public float armorMult = 0f;
    public float overallDamageMult = 1f;

    Collider col;

    // Armor multiplier examples:
    // 0.0 = Armor has no effect.
    // 0.1 = Armor takes 90% of the damage. Character takes 10%.
    // 0.5 = Armor takes 50% of the damage. Character takes 50%.
    // 0.75 = Armor takes 25% of the damage. Character takes 75%.
    // 1.0 = Armor takes 100% of the damage.

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
        // Health minimum
        if(health < 0)
        {
            health = 0;
        }

        if(health <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        if(isDead == true)
        {
            col.enabled = false;
        }

        // Armor minimum
        if(armor < 0)
        {
            armor = 0;
        }
    }

    public void TakeDamage(int amount, bool ignoreArmor)
    {
        if (StaticClass.gameState == 0)
        {
            amount = Mathf.RoundToInt(amount * overallDamageMult);

            if (ignoreArmor == false && armor > 0)
            {
                int armorDamage = Mathf.RoundToInt(amount * (1 - armorMult));
                health -= Mathf.RoundToInt(amount * armorMult);

                for (int i = 0; i < armorDamage; i++)
                {
                    if (armor > 0)
                    {
                        armor--;
                    }
                    else
                    {
                        health--;
                    }
                }
            }
            else
            {
                health -= amount;
            }

            // If this character is an enemy, start its pain coroutine
            if (GetComponent<Enemy>() != null)
            {
                StartCoroutine(GetComponent<Enemy>().Pain());
            }

            // If this character is a player, start its pain coroutine
            if (GetComponent<Player>() != null)
            {
                GetComponent<Player>().PlayerPain();
            }
        }
    }
}
