using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class TitleScreen : MonoBehaviour
{
    public Text versionText;
    public Image selectIcon;
    public GameObject sectionStart;
    public GameObject sectionChooseChapter;
    public GameObject sectionDifficulty;
    public GameObject sectionLoadGame;
    public Text[] loadGameSlotsText;

    void Start()
    {
        Time.timeScale = 1.0f;
        Player.scoreThisLevel = 0;
        StaticClass.currentChapter = 1;
        StaticClass.currentMap = 1;
        StaticClass.secretsDiscovered = 0;
        StaticClass.secretsTotal = 0;
        StaticClass.enemiesTotal = 0;
        StaticClass.loadSavedPlayerInfo = false;
        Player.timeSeconds = 0;
        Player.timeMinutes = 0;
        Cursor.lockState = CursorLockMode.None;
        versionText.text = "v " + Application.version.ToString();
        SectionStart();

        int i = 0;
        foreach (Text preview in loadGameSlotsText)
        {
            preview.text += " - ";

            if (PlayerPrefs.HasKey("slot" + i.ToString() + "_scene_name"))
            {
                preview.text += "(" + PlayerPrefs.GetString("slot" + i.ToString() + "_scene_name") + ", SCORE: " + PlayerPrefs.GetInt("slot" + i.ToString() + "_score").ToString() + ")";
            }
            else
            {
                preview.text += "(EMPTY)";
            }

            i++;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SectionStart()
    {
        sectionStart.SetActive(true);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        selectIcon.enabled = false;
    }

    public void SectionChooseChapter()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(true);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(false);
        selectIcon.enabled = false;
    }

    public void SectionDifficulty()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(true);
        sectionLoadGame.SetActive(false);
        selectIcon.enabled = false;
    }

    public void SectionLoadGame()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
        sectionLoadGame.SetActive(true);
        selectIcon.enabled = false;
    }

    public void SetChapter(int c)
    {
        StaticClass.currentChapter = c;
    }

    public void SetDifficulty(int d)
    {
        StaticClass.difficulty = d;
    }

    public void StartChapter()
    {
        SceneManager.LoadScene("C" + StaticClass.currentChapter.ToString() + "M1");
    }

    public void LoadGame(int slot)
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
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
