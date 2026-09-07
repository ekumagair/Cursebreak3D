using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Variables

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
    public bool triggersAutosave = false;

    private Player _player;

    #endregion

    #region Default Methods

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (StaticClass.loadSavedMapData == true)
        {
            if (_player.destroyedItemsPositions.Contains(transform.position.x.ToString() + transform.position.y.ToString() + transform.position.z.ToString()))
            {
                if (giveKey > 0)
                {
                    _player.keys[giveKey] = true;
                }

                Destroy(gameObject);
            }
        }
    }

    #endregion
}
