using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Text hpText;
    public Text armorText;
    public Text ammoText;
    public Text ammo1Text;
    public Text ammo2Text;

    public Image weaponImage;
    public Image invisOverlay;
    public Sprite[] weaponSprites;

    GameObject statTarget;
    Health targetHealth;
    Player targetPlayer;

    void Start()
    {
        statTarget = GameObject.FindGameObjectWithTag("Player");
        targetHealth = statTarget.GetComponent<Health>();

        if(statTarget.GetComponent<Player>() != null)
        {
            targetPlayer = statTarget.GetComponent<Player>();
        }
    }

    void Update()
    {
        hpText.text = targetHealth.health.ToString();
        armorText.text = targetHealth.armor.ToString();
        ammo1Text.text = "Plasma: " + targetPlayer.ammo[0].ToString() + " / " + targetPlayer.ammoLimit[0].ToString();
        ammo2Text.text = "Fire: " + targetPlayer.ammo[1].ToString() + " / " + targetPlayer.ammoLimit[1].ToString();
        weaponImage.sprite = weaponSprites[targetPlayer.currentWeapon];

        if (targetPlayer.weaponAmmoCost[targetPlayer.currentWeapon] > 0)
        {
            ammoText.text = targetPlayer.ammo[targetPlayer.weaponAmmoType[targetPlayer.currentWeapon]].ToString();
        }
        else
        {
            ammoText.text = "-";
        }
    }
}
