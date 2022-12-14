using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class HUD : MonoBehaviour
{
    [Header("Status Bar")]
    public Text hpText;
    public Text armorText;
    public Text ammoText;
    public Text ammoHeaderText;
    public Text ammo1Text;
    public Text ammo2Text;
    public Text ammo3Text;
    public Text messageText;
    public GameObject logMessagePrefab;

    [Header("Pause")]
    public Image pauseSelectIcon;
    public GameObject pauseGeneral;
    public GameObject pauseRoot;
    public GameObject sectionSelectSave;

    [Header("Minimap")]
    public Text minimapMapNumberDisplay;
    public Text minimapEnemyText;
    public Text minimapSecretsText;
    public Text minimapScoreText;
    public GameObject mapRoot;

    [Header("Weapons")]
    public Image weaponImage;
    public Sprite[] weaponSprites;
    public Image[] firstPersonSprites;
    public Image[] conditionOverlays;
    public Animator[] conditionOverlaysAnimators;
    public Color firstPersonSpritesInvisibleColor;

    public float messageTimer = 0f;
    public static int logCurrentPosition = 0;

    public static bool mapEnabled = false;

    GameObject statTarget;
    Health targetHealth;
    Player targetPlayer;

    void Start()
    {
        if (statTarget == null)
        {
            statTarget = GameObject.FindGameObjectWithTag("Player");
        }

        targetHealth = statTarget.GetComponent<Health>();
        logCurrentPosition = 0;
        messageTimer = 0f;
        mapEnabled = false;

        if (statTarget.GetComponent<Player>() != null)
        {
            targetPlayer = statTarget.GetComponent<Player>();
        }
        else
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
    }

    void Update()
    {
        hpText.text = targetHealth.health.ToString();
        armorText.text = targetHealth.armor.ToString();
        ammo1Text.text = "Plasma: " + targetPlayer.ammo[0].ToString() + "/" + targetPlayer.ammoLimit[0].ToString();
        ammo2Text.text = "Fire: " + targetPlayer.ammo[1].ToString() + "/" + targetPlayer.ammoLimit[1].ToString();
        ammo3Text.text = "Electricity: " + targetPlayer.ammo[2].ToString() + "/" + targetPlayer.ammoLimit[2].ToString();
        minimapMapNumberDisplay.text = "Chapter " + StaticClass.currentChapter.ToString() + " - Map " + StaticClass.currentMap.ToString();
        minimapEnemyText.text = "Foes: " + StaticClass.enemiesKilled.ToString() + "/" + StaticClass.enemiesTotal.ToString();
        minimapSecretsText.text = "Secrets: " + StaticClass.secretsDiscovered.ToString() + "/" + StaticClass.secretsTotal.ToString();
        minimapScoreText.text = "Score: " + Player.scoreThisLevel.ToString();
        weaponImage.sprite = weaponSprites[targetPlayer.currentWeapon];

        // Show score if recently collected item that gives you score. By default, show ammo.
        if (Player.gotScoreTimer == 0)
        {
            ammoHeaderText.text = "Munition";
            if (targetPlayer.weaponAmmoCost[targetPlayer.currentWeapon] > 0)
            {
                ammoText.text = targetPlayer.ammo[targetPlayer.weaponAmmoType[targetPlayer.currentWeapon]].ToString();
            }
            else
            {
                ammoText.text = "-";
            }
        }
        else
        {
            ammoHeaderText.text = "Score";
            ammoText.text = Player.scoreThisLevel.ToString();
        }

        // Message on the center of the screen.
        if(messageTimer > 0.0f)
        {
            messageTimer -= Time.deltaTime;
        }
        if(messageTimer < 0.0f)
        {
            messageTimer = 0.0f;
        }

        if(messageTimer > 0.0f && Time.timeScale != 0.0f)
        {
            messageText.enabled = true;
        }
        else
        {
            messageText.enabled = false;
        }

        // If player script is valid
        if(targetPlayer != null)
        {
            // For every first person sprite
            for (int i = 0; i < firstPersonSprites.Length; i++)
            {
                if (targetPlayer.conditionTimer[0] > 0)
                {
                    // Apply invisibility color
                    firstPersonSprites[i].color = firstPersonSpritesInvisibleColor;
                }
                else
                {
                    firstPersonSprites[i].color = Color.white;
                }

                // Hide sprites if the player is dead
                if(targetHealth.health <= 0)
                {
                    firstPersonSprites[i].enabled = false;
                }
            }
        }

        // If paused
        if(Time.timeScale == 0.0f)
        {
            pauseGeneral.SetActive(true);
            mapEnabled = false;
        }
        else
        {
            pauseGeneral.SetActive(false);
            pauseRoot.SetActive(false);
            sectionSelectSave.SetActive(false);
        }

        // Toggle minimap
        if(Input.GetKeyDown(KeyCode.Tab) && StaticClass.gameState == 0 && StaticClass.minimapType != 0)
        {
            mapEnabled = !mapEnabled;
        }
        mapRoot.SetActive(mapEnabled);
    }

    public void PauseEnd()
    {
        targetPlayer.PauseEnd();
        StaticClass.gameState = 0;
    }

    public void GoToPauseRoot()
    {
        pauseRoot.SetActive(true);
        sectionSelectSave.SetActive(false);
        pauseSelectIcon.enabled = false;
    }

    public void GoToSelectSave()
    {
        pauseRoot.SetActive(false);
        sectionSelectSave.SetActive(true);
        pauseSelectIcon.enabled = false;
    }

    public void SaveToSlot(int s)
    {
        SaveData(s);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void HudMessage(string message, float duration)
    {
        messageText.text = message;
        messageTimer = duration;
    }

    public void HudAddLog(string message)
    {
        var msg = Instantiate(logMessagePrefab, gameObject.transform);
        msg.GetComponent<Text>().text = message;
        msg.GetComponent<RectTransform>().position = new Vector3(0, (-32 * logCurrentPosition) + 800, 0);
        logCurrentPosition++;

        if (logCurrentPosition > 4)
        {
            HudMoveUpLog();
        }

        Debug.Log(message);
        Debug.Log("Log position: " + HUD.logCurrentPosition);
    }

    public void HudMoveUpLog()
    {
        HUD.logCurrentPosition--;
        LogMessageScript[] logMsg = FindObjectsOfType<LogMessageScript>();
        foreach (LogMessageScript lm in logMsg)
        {
            lm.MoveUp();
        }

        if (logCurrentPosition < 0)
        {
            logCurrentPosition = 0;
        }

        Debug.Log("Log position: " + HUD.logCurrentPosition);
    }

    public void SaveData(int slot)
    {
        PlayerPrefs.SetString("slot" + slot.ToString() + "_scene_name", SceneManager.GetActiveScene().name);
        PlayerPrefs.SetInt("slot" + slot.ToString() + "_difficulty", StaticClass.difficulty);
        PlayerPrefs.SetInt("slot" + slot.ToString() + "_score", Player.score);
        PlayerPrefs.SetInt("slot" + slot.ToString() + "_health", targetHealth.health);
        PlayerPrefs.SetInt("slot" + slot.ToString() + "_armor", targetHealth.armor);
        PlayerPrefs.SetFloat("slot" + slot.ToString() + "_armor_mult", targetHealth.armorMult);

        for (int i = 0; i < targetPlayer.ammo.Length; i++)
        {
            PlayerPrefs.SetInt("slot" + slot.ToString() + "_ammo" + i.ToString(), targetPlayer.ammo[i]);
        }

        for (int x = 0; x < targetPlayer.weaponsUnlocked.Length; x++)
        {
            PlayerPrefs.SetInt("slot" + slot.ToString() + "_weapon_unlocked" + x.ToString(), Convert.ToInt32(targetPlayer.weaponsUnlocked[x]));
        }

        Debug.Log("Saved game on slot " + slot.ToString());
    }
}
