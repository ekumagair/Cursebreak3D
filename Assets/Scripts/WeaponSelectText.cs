using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSelectText : MonoBehaviour
{
    #region Variables

    public int id;
    public int ammoType;

    private Text _txt;
    private GameObject _playerObject;
    private Player _player;

    #endregion

    #region Default Methods

    void Start()
    {
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = _playerObject.GetComponent<Player>();
        _txt = GetComponent<Text>();
    }

    void Update()
    {
        if (_player.currentWeapon == id)
        {
            // If this is the currently selected weapon.
            _txt.color = Color.yellow;
        }
        else if (_player.weaponsUnlocked[id] == true)
        {
            if (ammoType > -1)
            {
                if (_player.ammo[ammoType] > 0)
                {
                    // If you have the weapon and the ammo.
                    _txt.color = Color.white;
                }
                else
                {
                    // If you have the weapon, but not the ammo.
                    _txt.color = new Color(0.7f, 0.7f, 0.7f, 1);
                }
            }
            else
            {
                // If you have the weapon and it doesn't need ammo.
                _txt.color = Color.white;
            }
        }
        else
        {
            // If you don't have the weapon.
            _txt.color = new Color(0.4f, 0.4f, 0.4f, 1);
        }
    }

    #endregion
}
