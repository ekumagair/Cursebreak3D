using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSectionSelectLevel : MonoBehaviour
{
    #region Variables

    public AudioSource clickSound;
    public GameObject section;

    [Space]

    public Text chapterText;
    public Button chapterAdd;
    public Button chapterSubtract;

    [Space]

    public Text mapText;
    public Button mapAdd;
    public Button mapSubtract;

    [Space]

    public Toggle toggleAllWeapons;
    public Toggle toggleFullArmor;

    public static bool levelWarp = false;
    public static bool allWeapons = false;
    public static bool fullArmor = false;

    #endregion

    #region Default Methods

    void Update()
    {
        if (StaticClass.currentChapter <= 1)
        {
            chapterSubtract.interactable = false;
        }
        else
        {
            chapterSubtract.interactable = true;
        }
        if (StaticClass.currentChapter >= 3)
        {
            chapterAdd.interactable = false;
        }
        else
        {
            chapterAdd.interactable = true;
        }

        if (StaticClass.currentMap <= 1)
        {
            mapSubtract.interactable = false;
        }
        else
        {
            mapSubtract.interactable = true;
        }
        if (StaticClass.currentMap >= 5)
        {
            mapAdd.interactable = false;
        }
        else
        {
            mapAdd.interactable = true;
        }

        chapterText.text = "Chapter " + StaticClass.currentChapter.ToString();
        mapText.text = "Map " + StaticClass.currentMap.ToString();
    }

    #endregion

    #region Configurations

    public void ResetConfigurations()
    {
        allWeapons = false;
        fullArmor = false;

        toggleAllWeapons.isOn = false;
        toggleFullArmor.isOn = false;
    }

    public void SetAllWeaponsToggle()
    {
        allWeapons = toggleAllWeapons.isOn;
    }

    public void SetFullArmorToggle()
    {
        fullArmor = toggleFullArmor.isOn;
    }

    #endregion

    #region Sounds

    public void PlayClickSound()
    {
        if (section.activeInHierarchy == true && clickSound.gameObject.activeInHierarchy == true)
        {
            clickSound.Play();
        }
    }

    #endregion
}
