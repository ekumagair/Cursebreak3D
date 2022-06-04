using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectText : MonoBehaviour
{
    public int id;
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
            txt.color = Color.yellow;
        }
        else if(player.weaponsUnlocked[id] == true)
        {
            txt.color = Color.white;
        }
        else
        {
            txt.color = Color.gray;
        }
    }
}
