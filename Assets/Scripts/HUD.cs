using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class HUD : MonoBehaviour
{
    [Header("Target Texture")]
    public RawImage targetTextureImage;

    [Header("Overlays")]
    public RectTransform overlaysRoot;

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
    public GameObject sectionOptions;

    [Header("Select save slot")]
    public Text[] saveSlotText;

    [Header("Minimap")]
    public Text minimapMapNumberDisplay;
    public Text minimapEnemyText;
    public Text minimapSecretsText;
    public Text minimapCurrentScoreText;
    public Text minimapTotalScoreText;
    public GameObject mapRoot;

    [Header("Weapons")]
    public Image weaponImage;
    public Sprite[] weaponSprites;
    public Image[] firstPersonSprites;
    public Image[] conditionOverlays;
    public Animator[] conditionOverlaysAnimators;
    public Color firstPersonSpritesInvisibleColor;

    [Header("Loading")]
    public GameObject loadingScreen;

    [Header("Messages")]
    public float messageTimer = 0f;
    public static int logCurrentPosition = 0;

    public static bool minimapEnabled = false;

    GameObject statTarget;
    Health targetHealth;
    Player targetPlayer;

    void Awake()
    {
        if (StaticClass.pendingLoad == -1)
        {
            loadingScreen.SetActive(false);
        }
        else
        {
            loadingScreen.SetActive(true);
        }
    }

    void Start()
    {
        if (statTarget == null)
        {
            statTarget = GameObject.FindGameObjectWithTag("Player");
        }

        targetHealth = statTarget.GetComponent<Health>();
        logCurrentPosition = 0;
        messageTimer = 0f;
        minimapEnabled = false;

        if (statTarget.GetComponent<Player>() != null)
        {
            targetPlayer = statTarget.GetComponent<Player>();
        }
        else
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }

        // Tell the player if a save slot is empty.
        for (int i = 0; i < saveSlotText.Length; i++)
        {
            // Check for save slot "i + 1" because save slot 0 is the auto save, which isn't an option here.
            if (SaveSystem.SaveExists(i + 1, "player") == false)
            {
                saveSlotText[i].text += " (EMPTY)";
            }
            else
            {
                PlayerData data = SaveSystem.LoadPlayer(i + 1);
                saveSlotText[i].text += " (MAP: " + data.scene.ToString() + ")";
            }
        }
    }

    void Update()
    {
        if (targetHealth.health >= 0)
        {
            hpText.text = targetHealth.health.ToString();
        }
        else
        {
            // Don't display negative numbers for health.
            hpText.text = "0";
        }

        if (targetHealth.armor >= 0)
        {
            armorText.text = targetHealth.armor.ToString();
        }
        else
        {
            // Don't display negative numbers for armor.
            armorText.text = "0";
        }

        ammo1Text.text = "Plasma: " + targetPlayer.ammo[0].ToString() + "/" + targetPlayer.ammoLimit[0].ToString();
        ammo2Text.text = "Fire: " + targetPlayer.ammo[1].ToString() + "/" + targetPlayer.ammoLimit[1].ToString();
        ammo3Text.text = "Electricity: " + targetPlayer.ammo[2].ToString() + "/" + targetPlayer.ammoLimit[2].ToString();
        minimapMapNumberDisplay.text = "Chapter " + StaticClass.currentChapter.ToString() + " - Map " + StaticClass.currentMap.ToString();
        minimapEnemyText.text = "Foes: " + StaticClass.enemiesKilled.ToString() + "/" + StaticClass.enemiesTotal.ToString();
        minimapSecretsText.text = "Secrets: " + StaticClass.secretsDiscovered.ToString() + "/" + StaticClass.secretsTotal.ToString();
        minimapCurrentScoreText.text = "Current score: " + Player.scoreThisLevel.ToString();
        minimapTotalScoreText.text = "Total score: " + (Player.score + Player.scoreThisLevel).ToString();
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
        if (messageTimer > 0.0f)
        {
            messageTimer -= Time.deltaTime;
        }
        if (messageTimer < 0.0f)
        {
            messageTimer = 0.0f;
        }

        if (messageTimer > 0.0f && Time.timeScale != 0.0f)
        {
            messageText.enabled = true;
        }
        else
        {
            messageText.enabled = false;
        }

        // If player script is valid
        if (targetPlayer != null)
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
        if (Time.timeScale == 0.0f)
        {
            pauseGeneral.SetActive(true);
            minimapEnabled = false;
        }
        else
        {
            pauseGeneral.SetActive(false);
            pauseRoot.SetActive(false);
            sectionSelectSave.SetActive(false);
            sectionOptions.SetActive(false);
        }

        // Toggle minimap
        if (Input.GetKeyDown(KeyCode.Tab) && StaticClass.gameState == 0 && StaticClass.minimapType != 0)
        {
            minimapEnabled = !minimapEnabled;
        }
        mapRoot.SetActive(minimapEnabled);
    }

    public void PauseEnd()
    {
        targetPlayer.PauseEnd();
        StaticClass.gameState = 0;
        SaveSystem.SaveGlobal();
    }

    public void GoToPauseRoot()
    {
        pauseRoot.SetActive(true);
        sectionSelectSave.SetActive(false);
        sectionOptions.SetActive(false);
        pauseSelectIcon.enabled = false;
        SaveSystem.SaveGlobal();
    }

    public void GoToSelectSave()
    {
        pauseRoot.SetActive(false);
        sectionSelectSave.SetActive(true);
        sectionOptions.SetActive(false);
        pauseSelectIcon.enabled = false;
        SaveSystem.SaveGlobal();
    }

    public void GoToOptions()
    {
        pauseRoot.SetActive(false);
        sectionSelectSave.SetActive(false);
        sectionOptions.SetActive(true);
        pauseSelectIcon.enabled = false;
    }

    public void SaveToSlot(int s)
    {
        SaveSystem.SaveGame(s);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    // Message on the center of the screen.
    public void HudMessage(string message, float duration)
    {
        messageText.text = message;
        messageTimer = duration;
    }

    // Messages on the top left part of the screen.
    public void HudAddLog(string message)
    {
        var msg = Instantiate(logMessagePrefab, gameObject.transform);
        msg.GetComponent<Text>().text = message;
        msg.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -32 * logCurrentPosition, 0);
        logCurrentPosition++;

        if (logCurrentPosition > 4)
        {
            HudMoveUpLog();
        }

        if (StaticClass.debug == true)
        {
            Debug.Log(message);
            Debug.Log("Log position: " + logCurrentPosition);
        }
    }

    public void HudMoveUpLog()
    {
        logCurrentPosition--;
        LogMessageScript[] logMsg = FindObjectsOfType<LogMessageScript>();
        foreach (LogMessageScript lm in logMsg)
        {
            lm.MoveUp();
        }

        if (logCurrentPosition < 0)
        {
            logCurrentPosition = 0;
        }

        if (StaticClass.debug == true)
        {
            Debug.Log("Log position: " + logCurrentPosition);
        }
    }
}
