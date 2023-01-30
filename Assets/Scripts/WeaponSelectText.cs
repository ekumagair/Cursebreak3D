using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectText : MonoBehaviour
{
    public int id;
    public int ammoType;
    Text txt;
    GameObject playerObject;
    Player player;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.GetComponent<Player>();
        txt = GetComponent<Text>();
    }

    private void Update()
    {
        if(player.currentWeapon == id)
        {
            // If this is the currently selected weapon.
            txt.color = Color.yellow;
        }
        else if(player.weaponsUnlocked[id] == true)
        {
            if (ammoType > -1)
            {
                if (player.ammo[ammoType] > 0)
                {
                    // If you have the weapon and the ammo.
                    txt.color = Color.white;
                }
                else
                {
                    // If you have the weapon, but not the ammo.
                    txt.color = new Color(0.7f, 0.7f, 0.7f, 1);
                }
            }
            else
            {
                // If you have the weapon and it doesn't need ammo.
                txt.color = Color.white;
            }
        }
        else
        {
            // If you don't have the weapon.
            txt.color = new Color(0.4f, 0.4f, 0.4f, 1);
        }
    }
}
