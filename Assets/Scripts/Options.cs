using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [Header("Mouse sensitivity")]
    public Slider mouseSensitivitySlider;
    public Text mouseSensitivityValue;

    [Header("Music")]
    public Slider musicSlider;
    public Text musicValue;

    [Header("Sound")]
    public Slider soundSlider;
    public Text soundValue;

    [Header("Crosshair")]
    public Text crosshairText;
    public Button crosshairAdd;
    public Button crosshairSubtract;

    [Header("Flashing")]
    public Text flashingText;
    public Button flashingAdd;
    public Button flashingSubtract;

    [Header("Resolution")]
    public Text resolutionText;
    public Button resolutionAdd;
    public Button resolutionSubtract;

    public static float mouseSensitivity = 1.0f;
    public static float musicVolume = 0.5f;
    public static float soundVolume = 1.0f;
    public static int flashingEffects = 0;
    public static int gameResolution = 2;

    void Start()
    {
        if (File.Exists(SaveSystem.GlobalSavePath()))
        {
            GlobalData data = SaveSystem.GetSavedGlobal();

            mouseSensitivity = data.mouseSensitivity;
            musicVolume = data.musicVolume;
            soundVolume = data.soundVolume;
            Crosshair.sprite = data.crosshairSprite;
            flashingEffects = data.flashingEffects;
        }

        mouseSensitivitySlider.value = mouseSensitivity * 10;
        musicSlider.value = musicVolume * 100;
        soundSlider.value = soundVolume * 100;
    }

    void Update()
    {
        // Slider-controlled settings.
        mouseSensitivityValue.text = "(" + mouseSensitivity + "x)";
        musicValue.text = "(" + musicSlider.value + "%)";
        soundValue.text = "(" + soundSlider.value + "%)";

        // Crosshair settings.
        crosshairText.text = "Crosshair: ";
        switch (Crosshair.sprite)
        {
            case 0:
                crosshairText.text += "None";
                break;
            case 1:
                crosshairText.text += "Dot";
                break;
            case 2:
                crosshairText.text += "Circle";
                break;
            case 3:
                crosshairText.text += "Cross";
                break;
            case 4:
                crosshairText.text += "X";
                break;
            default:
                break;
        }

        if (Crosshair.sprite >= 4)
        {
            crosshairAdd.interactable = false;
        }
        else
        {
            crosshairAdd.interactable = true;
        }

        if (Crosshair.sprite <= 0)
        {
            crosshairSubtract.interactable = false;
        }
        else
        {
            crosshairSubtract.interactable = true;
        }

        // Flashing effects settings.
        flashingText.text = "Flashes: ";
        switch (flashingEffects)
        {
            case 0:
                flashingText.text += "All effects";
                break;
            case 1:
                flashingText.text += "Pickups only";
                break;
            case 2:
                flashingText.text += "Damage only";
                break;
            case 3:
                flashingText.text += "Power-ups only";
                break;
            case 4:
                flashingText.text += "None";
                break;
            default:
                break;
        }

        if (flashingEffects >= 4)
        {
            flashingAdd.interactable = false;
        }
        else
        {
            flashingAdd.interactable = true;
        }

        if (flashingEffects <= 0)
        {
            flashingSubtract.interactable = false;
        }
        else
        {
            flashingSubtract.interactable = true;
        }

        // Game resolution settings.
        resolutionText.text = "";
        switch (gameResolution)
        {
            case 0:
                resolutionText.text += "1280x800";
                break;
            case 1:
                resolutionText.text += "1728x1080";
                break;
            case 2:
                resolutionText.text += "1920x1080";
                break;
            case 3:
                resolutionText.text += "3840x2160";
                break;
            default:
                resolutionText.text += "1280x800";
                break;
        }

        if (gameResolution >= 3)
        {
            resolutionAdd.interactable = false;
        }
        else
        {
            resolutionAdd.interactable = true;
        }

        if (gameResolution <= 0)
        {
            resolutionSubtract.interactable = false;
        }
        else
        {
            resolutionSubtract.interactable = true;
        }
    }

    public void SetMouseSensitivity()
    {
        mouseSensitivity = mouseSensitivitySlider.value / 10;
    }

    public void SetMusicVolume()
    {
        musicVolume = musicSlider.value / 100;
    }

    public void SetSoundVolume()
    {
        soundVolume = soundSlider.value / 100;
    }

    public void ChangeCrosshair(int increment)
    {
        Crosshair.sprite += increment;
    }

    public void ChangeFlashingFX(int increment)
    {
        flashingEffects += increment;
    }

    public void ChangeResolution(int increment)
    {
        gameResolution += increment;
        SetResolution();
    }

    public static void SetResolution()
    {
        switch (gameResolution)
        {
            case 0:
                Screen.SetResolution(1280, 800, true);
                break;

            case 1:
                Screen.SetResolution(1728, 1080, true);
                break;

            case 2:
                Screen.SetResolution(1920, 1080, true);
                break;

            case 3:
                Screen.SetResolution(3840, 2160, true);
                break;

            default:
                Screen.SetResolution(1280, 800, true);
                break;
        }

        if (FindObjectOfType<Player>() != null)
        {
            FindObjectOfType<Player>().SetRenderTexture();
        }
    }

    public static void ResetOptions()
    {
        mouseSensitivity = 1.0f;
        musicVolume = 0.5f;
        soundVolume = 1.0f;
        Crosshair.sprite = 0;
        flashingEffects = 0;
        gameResolution = 2;

        SaveSystem.SaveGlobal();

        if (StaticClass.debug == true)
        {
            Debug.Log("Reset options.");
        }
    }
}
