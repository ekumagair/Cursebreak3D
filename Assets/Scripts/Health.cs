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

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
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

            if (GetComponent<Enemy>() != null)
            {
                StartCoroutine(GetComponent<Enemy>().Pain());
            }
        }
    }
}
