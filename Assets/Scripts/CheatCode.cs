using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatCode : MonoBehaviour
{
    [Header("Effects")]
    public int giveWeapon = 0;
    public int giveArmor = 0;
    public float giveArmorMult = 0.5f;
    public int giveKey = 0;
    public bool giveFullAmmo = false;
    public bool giveLevelWin = false;
    public string goToScene = "";

    [Header("Cheat Properties")]
    public bool once = true;
    public bool playSound = false;
    public KeyCode[] buttons;
    public int currentButton;

    Player playerScript;

    void Start()
    {
        currentButton = 0;
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && Event.current.type == EventType.KeyUp)
        {
            if(buttons[currentButton] == e.keyCode)
            {
                // Check next key.
                currentButton++;

                // If typed every key.
                if(currentButton == buttons.Length)
                {
                    currentButton = 0;

                    if(giveWeapon > 0)
                    {
                        playerScript.weaponsUnlocked[giveWeapon] = true;
                        playerScript.ammo[playerScript.weaponAmmoType[giveWeapon]] += 20;
                    }
                    if (giveWeapon == -1)
                    {
                        for (int i = 0; i < playerScript.weaponsUnlocked.Length; i++)
                        {
                            playerScript.weaponsUnlocked[i] = true;
                            playerScript.ammo[playerScript.weaponAmmoType[i]] += 20;
                        }
                    }
                    if (giveArmor > 0)
                    {
                        playerScript.GetComponent<Health>().armor += giveArmor;
                        playerScript.GetComponent<Health>().armorMult = giveArmorMult;
                    }
                    if(giveKey > 0)
                    {
                        playerScript.keys[giveKey] = true;
                    }
                    if (giveKey == -1)
                    {
                        for (int i = 0; i < playerScript.keys.Length; i++)
                        {
                            playerScript.keys[i] = true;
                        }
                    }
                    if (giveFullAmmo == true)
                    {
                        for (int i = 0; i < playerScript.ammoLimit.Length; i++)
                        {
                            playerScript.ammo[i] = playerScript.ammoLimit[i];
                        }
                    }
                    if(giveLevelWin == true)
                    {
                        //StartCoroutine(playerScript.Exit(playerScript.deathFadeObject));
                        playerScript.StartCoroutine(playerScript.Exit(null));
                        //SceneManager.LoadScene("Intermission");
                    }
                    if(goToScene != "")
                    {
                        SceneManager.LoadScene(goToScene);
                    }

                    if(playSound == true)
                    {
                        GetComponent<AudioSource>().Play();
                    }
                    if(once == true)
                    {
                        Destroy(gameObject);
                    }

                }
            }
            else
            {
                currentButton = 0;
            }
        }
    }
}
