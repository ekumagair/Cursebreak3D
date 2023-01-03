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
    public Text[] loadGameSlotsText;
    public string[] loadGameTextsDefault = new string[4];

    void Start()
    {
        Time.timeScale = 1.0f;
        StaticClass.ResetStats();
        Cursor.lockState = CursorLockMode.None;
        versionText.text = "v " + Application.version.ToString();

        for (int i = 0; i < loadGameSlotsText.Length; i++)
        {
            loadGameTextsDefault[i] = loadGameSlotsText[i].text;
        }

        if (PlayerPrefs.HasKey("global_mouse_sensitivity"))
        {
            Options.mouseSensitivity = PlayerPrefs.GetFloat("global_mouse_sensitivity");
            Options.musicVolume = PlayerPrefs.GetFloat("global_music_volume");
            Options.soundVolume = PlayerPrefs.GetFloat("global_sfx_volume");
        }

        SectionStart();
        SetSaveSlotsTexts();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void SetSaveSlotsTexts()
    {
        int i = 0;
        foreach (Text preview in loadGameSlotsText)
        {
            preview.text = loadGameTextsDefault[i];

            Button slotButton = preview.transform.parent.gameObject.GetComponent<Button>();
            EventTrigger eventTrigger = preview.transform.parent.gameObject.GetComponent<EventTrigger>();
            preview.text += " - ";

            if (PlayerPrefs.HasKey("slot" + i.ToString() + "_scene_name"))
            {
                preview.text += "(" + PlayerPrefs.GetString("slot" + i.ToString() + "_scene_name") + ", SCORE: " + PlayerPrefs.GetInt("slot" + i.ToString() + "_score").ToString() + ")";
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
        SectionAny();
    }

    public void SectionDifficulty()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(true);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(false);
        SectionAny();
    }

    public void SectionLoadGame()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(true);
        sectionOptions.SetActive(false);
        SectionAny();
    }

    public void SectionOptions()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        sectionOptions.SetActive(true);
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

    public void StartChapter(bool story)
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

    public void LoadGame(int slot)
    {
        if (PlayerPrefs.HasKey("slot" + slot.ToString() + "_scene_name"))
        {
            StaticClass.difficulty = PlayerPrefs.GetInt("slot" + slot.ToString() + "_difficulty");
            Player.score = PlayerPrefs.GetInt("slot" + slot.ToString() + "_score");
            Player.savedHealth = PlayerPrefs.GetInt("slot" + slot.ToString() + "_health");
            Player.savedArmor = PlayerPrefs.GetInt("slot" + slot.ToString() + "_armor");
            Player.savedArmorMult = PlayerPrefs.GetFloat("slot" + slot.ToString() + "_armor_mult");

            for (int i = 0; i < Player.savedAmmo.Length; i++)
            {
                Player.savedAmmo[i] = PlayerPrefs.GetInt("slot" + slot.ToString() + "_ammo" + i.ToString());
            }

            for (int i = 0; i < Player.savedWeaponsUnlocked.Length; i++)
            {
                Player.savedWeaponsUnlocked[i] = Convert.ToBoolean(PlayerPrefs.GetInt("slot" + slot.ToString() + "_weapon_unlocked" + i.ToString()));
            }

            Debug.Log("Loaded game on slot " + slot.ToString());

            StaticClass.loadSavedPlayerInfo = true;

            SceneManager.LoadScene(PlayerPrefs.GetString("slot" + slot.ToString() + "_scene_name"));
        }
    }

    public void DeleteSave(int slot)
    {
        PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_scene_name");
        PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_difficulty");
        PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_score");
        PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_health");
        PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_armor");
        PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_armor_mult");

        for (int i = 0; i < Player.savedAmmo.Length; i++)
        {
            PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_ammo" + i.ToString());
        }

        for (int i = 0; i < Player.savedWeaponsUnlocked.Length; i++)
        {
            PlayerPrefs.DeleteKey("slot" + slot.ToString() + "_weapon_unlocked" + i);
        }

        Debug.Log("Deleted save on slot " + slot.ToString());

        SetSaveSlotsTexts();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
