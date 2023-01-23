using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

public class TitleScreen : MonoBehaviour
{
    public Text versionText;
    public Image selectIcon;
    public GameObject sectionStart;
    public GameObject sectionChooseChapter;
    public GameObject sectionDifficulty;
    public GameObject sectionLoadGame;
    public GameObject sectionOptions;
    public GameObject sectionSelectLevel;
    public Text[] loadGameSlotsText;
    
    string[] loadGameTextsDefault = new string[4];

    public static bool startFromChapterSelect = false;

    void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        MenuSectionSelectLevel.levelWarp = false;
        versionText.text = "v " + Application.version.ToString();

        // Sets default load game button text to the string set in the editor.
        for (int i = 0; i < loadGameSlotsText.Length; i++)
        {
            loadGameTextsDefault[i] = loadGameSlotsText[i].text;
        }

        // Load unlocked chapter varaible.
        if(PlayerPrefs.HasKey("global_unlocked_chapter"))
        {
            StaticClass.unlockedChapter = PlayerPrefs.GetInt("global_unlocked_chapter");
        }

        // Setting the unlocked chapter variable to less than 1 makes the game impossible to start. Don't allow this.
        if(StaticClass.unlockedChapter < 1)
        {
            StaticClass.unlockedChapter = 1;
            PlayerPrefs.SetInt("global_unlocked_chapter", 1);
            Debug.LogWarning("unlockedChapter variable was lower than 1! Resetting to default.");
        }

        if (startFromChapterSelect == false)
        {
            SectionStart();
        }
        else
        {
            SectionChooseChapter();
            startFromChapterSelect = false;
        }

        SetSaveSlotsTexts();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        if(StaticClass.debug == true)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                SceneManager.LoadScene("LevelSelect");
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                StaticClass.unlockedChapter--;
                Debug.Log("Unlocked chapter: " + StaticClass.unlockedChapter);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                StaticClass.unlockedChapter++;
                Debug.Log("Unlocked chapter: " + StaticClass.unlockedChapter);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.N))
                {
                    SaveSystem.LoadGame(0);
                }
            }
            if (Input.GetKey(KeyCode.Delete) && Input.GetKeyDown(KeyCode.Backspace))
            {
                PlayerPrefs.DeleteAll();
                SaveSystem.DeleteSave(0, "player");
                SaveSystem.DeleteSave(1, "player");
                SaveSystem.DeleteSave(2, "player");
                SaveSystem.DeleteSave(3, "player");
                StaticClass.ResetStats(true);
                StaticClass.unlockedChapter = 1;
                Options.ResetOptions();
                PlayerPrefs.Save();

                SetSaveSlotsTexts();
                Debug.Log("Cleared all saved data!");

                if(StaticClass.ignoreUnlockedChapter)
                {
                    Debug.Log("ignoreUnlockedChapter is set to true. All chapters are still unlocked.");
                }
            }
        }
    }

    void SetSaveSlotsTexts()
    {
        // Adds the save slot information to the button's text.
        int i = 0;
        foreach (Text preview in loadGameSlotsText)
        {
            // Must reset the button's text before adding the information.
            preview.text = loadGameTextsDefault[i];

            Button slotButton = preview.transform.parent.gameObject.GetComponent<Button>();
            EventTrigger eventTrigger = preview.transform.parent.gameObject.GetComponent<EventTrigger>();
            preview.text += " - ";

            if (SaveSystem.SaveExists(i, "player"))
            {
                preview.text += "(" + PlayerPrefs.GetString(StaticClass.SLOT_PREFIX + i.ToString() + "_scene_name") + ", SCORE: " + (PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + i.ToString() + "_scoreThisLevel") + PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + i.ToString() + "_score")).ToString() + ")";
                slotButton.interactable = true;
                eventTrigger.enabled = true;
            }
            else
            {
                preview.text += "(EMPTY)";
                slotButton.interactable = false;
                eventTrigger.enabled = false;
            }

            i++;
        }
    }

    public void SectionStart()
    {
        sectionStart.SetActive(true);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(false);
        sectionSelectLevel.SetActive(false);
        StaticClass.loadSavedPlayerInfo = false;
        SectionAny();
    }

    public void SectionChooseChapter()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(true);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(false);
        sectionSelectLevel.SetActive(false);
        StaticClass.ResetStats(true);
        MenuSectionSelectLevel.levelWarp = false;
        StaticClass.currentMap = 1;
        SectionAny();
    }

    public void SectionDifficulty()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(true);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(false);
        sectionSelectLevel.SetActive(false);
        SectionAny();
    }

    public void SectionLoadGame()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(true);
        sectionOptions.SetActive(false);
        sectionSelectLevel.SetActive(false);
        StaticClass.ResetStats(true);
        SectionAny();
    }

    public void SectionOptions()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(true);
        sectionSelectLevel.SetActive(false);
        SectionAny();
    }

    public void SectionSelectLevel()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(false);
        sectionSelectLevel.SetActive(true);
        MenuSectionSelectLevel.levelWarp = true;
        StaticClass.currentChapter = 1;
        StaticClass.currentMap = 1;
        SectionAny();
    }

    void SectionAny()
    {
        selectIcon.enabled = false;
        SetSaveSlotsTexts();
        PlayerPrefs.Save();
    }

    public void SetChapter(int c)
    {
        StaticClass.currentChapter = c;
    }

    public void SetDifficulty(int d)
    {
        StaticClass.difficulty = d;
    }

    public void AddChapter(int increment)
    {
        StaticClass.currentChapter += increment;
    }

    public void AddMap(int increment)
    {
        StaticClass.currentMap += increment;
    }

    public void StartChapter(bool story)
    {
        StaticClass.ResetStats(false);
        StaticClass.pendingLoad = -1;
        StaticClass.loadSavedPlayerInfo = false;
        StaticClass.loadSavedPlayerFullInfo = false;
        StaticClass.loadSavedMapData = false;
        startFromChapterSelect = false;

        if (MenuSectionSelectLevel.levelWarp == false)
        {
            if (story == false)
            {
                SceneManager.LoadScene("C" + StaticClass.currentChapter.ToString() + "M1");
            }
            else
            {
                switch (StaticClass.currentChapter)
                {
                    default:
                        StoryScreen.whichText = 0;
                        break;
                    case 1:
                        StoryScreen.whichText = 0;
                        break;
                    case 2:
                        StoryScreen.whichText = 1;
                        break;
                    case 3:
                        StoryScreen.whichText = 2;
                        break;
                }
                StoryScreen.goToTitle = false;
                SceneManager.LoadScene("Story");
            }
        }
        else
        {
            MenuSectionSelectLevel.levelWarp = false;
            SceneManager.LoadScene("C" + StaticClass.currentChapter.ToString() + "M" + StaticClass.currentMap.ToString());
        }
    }

    public void LoadGame(int slot)
    {
        StaticClass.ResetStats(true);
        string prefix = StaticClass.SLOT_PREFIX + slot.ToString();

        if (PlayerPrefs.HasKey(prefix + "_scene_name"))
        {
            SaveSystem.LoadGame(slot);
            
            StaticClass.difficulty = PlayerPrefs.GetInt(prefix + "_difficulty");
            Player.score = PlayerPrefs.GetInt(prefix + "_score");
            Player.scoreThisLevel = PlayerPrefs.GetInt(prefix + "_scoreThisLevel");/*
            Player.savedHealth = PlayerPrefs.GetInt(prefix + "_health");
            Player.savedArmor = PlayerPrefs.GetInt(prefix + "_armor");
            Player.savedArmorMult = PlayerPrefs.GetFloat(prefix + "_armor_mult");

            for (int i = 0; i < Player.savedAmmo.Length; i++)
            {
                Player.savedAmmo[i] = PlayerPrefs.GetInt(prefix + "_ammo" + i.ToString());
            }

            for (int i = 0; i < Player.savedWeaponsUnlocked.Length; i++)
            {
                Player.savedWeaponsUnlocked[i] = Convert.ToBoolean(PlayerPrefs.GetInt(prefix + "_weapon_unlocked" + i.ToString()));
            }

            Debug.Log("Loaded game on slot " + slot.ToString());

            StaticClass.loadSavedPlayerInfo = true;

            SceneManager.LoadScene(PlayerPrefs.GetString(prefix + "_scene_name"));*/
        }
    }

    public void DeleteSave(int slot)
    {
        SaveSystem.DeleteSave(slot, "player");

        string prefix = StaticClass.SLOT_PREFIX + slot.ToString();

        PlayerPrefs.DeleteKey(prefix + "_scene_name");
        PlayerPrefs.DeleteKey(prefix + "_difficulty");
        PlayerPrefs.DeleteKey(prefix + "_score");
        PlayerPrefs.DeleteKey(prefix + "_health");
        PlayerPrefs.DeleteKey(prefix + "_armor");
        PlayerPrefs.DeleteKey(prefix + "_armor_mult");

        for (int i = 0; i < Player.savedAmmo.Length; i++)
        {
            PlayerPrefs.DeleteKey(prefix + "_ammo" + i.ToString());
        }

        for (int i = 0; i < Player.savedWeaponsUnlocked.Length; i++)
        {
            PlayerPrefs.DeleteKey(prefix + "_weapon_unlocked" + i);
        }

        Debug.Log("Deleted save on slot " + slot.ToString());

        SetSaveSlotsTexts();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
