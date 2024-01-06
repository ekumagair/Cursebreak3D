using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;
using System.IO;

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
    public GameObject sectionCredits;
    public Text[] loadGameSlotsText;
    public GameObject[] loadGameDeleteButtons;
    public GameObject loadingScreen;
    
    private string[] _loadGameTextsDefault = new string[4];
    private float _deleteEverything = 0;
    private AudioSource _audioSource;

    public static bool startFromChapterSelect = false;

    void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        MenuSectionSelectLevel.levelWarp = false;
        _deleteEverything = 0;
        _audioSource = GetComponent<AudioSource>();
        versionText.text = "v " + Application.version.ToString();

        SaveSystem.LoadGlobal();
        Options.SetResolution();

        // Sets default load game button text to the string set in the editor.
        for (int i = 0; i < loadGameSlotsText.Length; i++)
        {
            _loadGameTextsDefault[i] = loadGameSlotsText[i].text;
        }

        // Load serialized unlocked chapter variable.
        if (File.Exists(SaveSystem.GlobalSavePath()))
        {
            StaticClass.unlockedChapter = SaveSystem.GetSavedGlobal().unlockedChapters;
        }

        // Setting the unlocked chapter variable to less than 1 makes the game impossible to start. Don't allow this.
        if (StaticClass.unlockedChapter < 1)
        {
            StaticClass.unlockedChapter = 1;
            Debug.LogWarning("unlockedChapter variable was lower than 1! Resetting to default.");
        }

        // Start this scene on the "select chapter" screen.
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

        if (StaticClass.debug == true)
        {
            Debug.LogWarning("**Debug mode is ON**");
        }
    }

    void Update()
    {
        // Quit the game by pressing esc.
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        // Delete all saved data. Must hold the "delete" key for 5 seconds and then press the "enter" key.
        if (Input.GetKey(KeyCode.Delete))
        {
            _deleteEverything += Time.deltaTime;

            if (_deleteEverything > 5 && Input.GetKeyDown(KeyCode.Return))
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

                if (StaticClass.ignoreUnlockedChapter == true && StaticClass.debug == true)
                {
                    Debug.Log("ignoreUnlockedChapter is set to true. All chapters are still unlocked.");
                }

                if (_audioSource != null && _audioSource.clip != null)
                {
                    _audioSource.Play();
                }

                SaveSystem.SaveGlobal();
                _deleteEverything = 0;
            }
        }
        else
        {
            _deleteEverything = 0;
        }

        if (StaticClass.debug == true)
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
            if (Input.GetKey(KeyCode.R))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Screen.SetResolution(1280, 800, true);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Screen.SetResolution(1280, 800, false);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Screen.SetResolution(1920, 1080, true);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    Screen.SetResolution(3840, 2160, true);
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
            preview.text = _loadGameTextsDefault[i];

            Button slotButton = preview.transform.parent.gameObject.GetComponent<Button>();
            EventTrigger eventTrigger = preview.transform.parent.gameObject.GetComponent<EventTrigger>();
            preview.text += " - ";
            preview.GetComponent<Shadow>().enabled = true;

            if (SaveSystem.SaveExists(i, "player"))
            {
                PlayerData data = SaveSystem.LoadPlayer(i);

                //preview.text += "(" + PlayerPrefs.GetString(StaticClass.SLOT_PREFIX + i.ToString() + "_scene_name") + ", SCORE: " + (PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + i.ToString() + "_scoreThisLevel") + PlayerPrefs.GetInt(StaticClass.SLOT_PREFIX + i.ToString() + "_score")).ToString() + ")"; Old player pref version.
                preview.text += "(" + data.scene.ToString() + ", SCORE: " + (data.score + data.scoreThisLevel).ToString() + ")";
                slotButton.interactable = true;
                eventTrigger.enabled = true;
                loadGameDeleteButtons[i].GetComponent<Button>().interactable = true;
                loadGameDeleteButtons[i].GetComponentInChildren<Text>().enabled = true;
                loadGameDeleteButtons[i].GetComponent<EventTrigger>().enabled = true;
            }
            else
            {
                preview.text += "(EMPTY)";
                slotButton.interactable = false;
                eventTrigger.enabled = false;
                loadGameDeleteButtons[i].GetComponent<Button>().interactable = false;
                loadGameDeleteButtons[i].GetComponentInChildren<Text>().enabled = false;
                loadGameDeleteButtons[i].GetComponent<EventTrigger>().enabled = false;
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
        sectionCredits.SetActive(false);
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
        sectionCredits.SetActive(false);
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
        sectionCredits.SetActive(false);
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
        sectionCredits.SetActive(false);
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
        sectionCredits.SetActive(false);
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
        sectionCredits.SetActive(false);
        MenuSectionSelectLevel.levelWarp = true;
        StaticClass.currentChapter = 1;
        StaticClass.currentMap = 1;
        SectionAny();
    }

    public void SectionCredits()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(false);
        sectionSelectLevel.SetActive(false);
        sectionCredits.SetActive(true);
        SectionAny();
    }

    void SectionAny()
    {
        selectIcon.enabled = false;
        SetSaveSlotsTexts();
        SaveSystem.SaveGlobal();
        //PlayerPrefs.Save();
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
        loadingScreen.SetActive(true);

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
        if (SaveSystem.SaveExists(slot, "player"))
        {
            loadingScreen.SetActive(true);
            StaticClass.ResetStats(true);
            SaveSystem.LoadGame(slot);
        }
        else if (StaticClass.debug == true)
        {
            Debug.Log("Title screen: No save found.");
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

        if (StaticClass.debug == true)
        {
            Debug.Log("Deleted save on slot " + slot.ToString());
        }

        SetSaveSlotsTexts();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
