using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int giveHealth = 0;
    public int giveWeapon = -1;
    public int giveAmmo = 0;
    public int giveAmmoType = 0;
    public int giveArmor = 0;
    public float giveArmorMult = 0;
    public int giveScore = 0;
    public int giveKey = -1;
    public int giveCondition = -1;
    public int giveConditionTimer = 0;
    public string logMessageOnCollect = "";
    public GameObject createOnCollect;
    public GameObject createOnCollectGameWorld;
    public bool canAlwaysCollect = false;
}
