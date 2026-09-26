using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CheatCode : MonoBehaviour
{
    [Header("Cheat Properties")]
    public bool once = true;
    public bool playSound = false;
    public bool debugOnly = false;

    [Header("Input")]
    public KeyCode[] buttons;
    public int currentButton;

    [Header("Effects")]
    public int giveWeapon = 0;
    public int giveArmor = 0;
    public float giveArmorMult = 0.5f;
    public float giveOverallMult = 1.0f;
    public int giveKey = 0;
    public bool giveFullAmmo = false;
    public bool giveLevelWin = false;
    public string goToScene = "";
    public UnityEvent onCheatTyped;

    private AudioSource _audioSource;
    private Player _playerScript;

    void Start()
    {
        currentButton = 0;
        _audioSource = GetComponent<AudioSource>();
        _playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void OnGUI()
    {
        Event e = Event.current;

        if (e.isKey && Event.current.type == EventType.KeyUp)
        {
            if (buttons[currentButton] == e.keyCode)
            {
                // Check next key.
                currentButton++;

                // If typed every key.
                if (currentButton == buttons.Length)
                {
                    if (debugOnly == false || (debugOnly == true && Debug.isDebugBuild == true))
                    {
                        ExecuteEffect();
                    }
                }
            }
            else
            {
                currentButton = 0;
            }
        }
    }

    private void ExecuteEffect()
    {
        currentButton = 0;

        if (giveWeapon > 0)
        {
            _playerScript.weaponsUnlocked[giveWeapon] = true;
            _playerScript.ammo[_playerScript.weaponAmmoType[giveWeapon]] += 20;
        }
        if (giveWeapon == -1)
        {
            for (int i = 0; i < _playerScript.weaponsUnlocked.Length; i++)
            {
                _playerScript.weaponsUnlocked[i] = true;
                _playerScript.ammo[_playerScript.weaponAmmoType[i]] += 20;
            }
        }
        if (giveArmor > 0)
        {
            _playerScript.HealthScript.armor += giveArmor;
            _playerScript.HealthScript.armorMult = giveArmorMult;
        }
        if (giveKey > 0)
        {
            _playerScript.keys[giveKey] = true;
        }
        if (giveKey == -1)
        {
            for (int i = 0; i < _playerScript.keys.Length; i++)
            {
                _playerScript.keys[i] = true;
            }
        }
        if (giveFullAmmo == true)
        {
            for (int i = 0; i < _playerScript.ammoLimit.Length; i++)
            {
                _playerScript.ammo[i] = _playerScript.ammoLimit[i];
            }
        }
        if (giveLevelWin == true)
        {
            _playerScript.StartCoroutine(_playerScript.Exit(null));
        }
        if (goToScene != "")
        {
            SceneManager.LoadScene(goToScene);
        }
        if (_playerScript != null)
        {
            _playerScript.HealthScript.overallDamageMult = giveOverallMult;
        }

        onCheatTyped?.Invoke();

        if (playSound == true && _audioSource != null)
        {
            _audioSource.Play();
        }
        if (once == true)
        {
            Destroy(gameObject);
        }
    }
}
