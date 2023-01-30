using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider mouseSensitivitySlider;
    public Slider musicSlider;
    public Slider soundSlider;

    public Text mouseSensitivityValue;
    public Text musicValue;
    public Text soundValue;

    public Text crosshairText;
    public Button crosshairAdd;
    public Button crosshairSubtract;

    public Text flashingText;
    public Button flashingAdd;
    public Button flashingSubtract;

    public static float mouseSensitivity = 1.0f;
    public static float musicVolume = 0.5f;
    public static float soundVolume = 1.0f;
    public static int flashingEffects = 0;

    void Start()
    {
        // Load saved settings.
        if (PlayerPrefs.HasKey("global_mouse_sensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("global_mouse_sensitivity");
            musicVolume = PlayerPrefs.GetFloat("global_music_volume");
            soundVolume = PlayerPrefs.GetFloat("global_sfx_volume");
            Crosshair.sprite = PlayerPrefs.GetInt("global_crosshair");
            flashingEffects = PlayerPrefs.GetInt("global_flashing");
        }

        mouseSensitivitySlider.value = mouseSensitivity * 10;
        musicSlider.value = musicVolume * 100;
        soundSlider.value = soundVolume * 100;
    }

    void Update()
    {
        // Slider-controlled settings
        mouseSensitivityValue.text = "(" + mouseSensitivity + "x)";
        musicValue.text = "(" + musicSlider.value + "%)";
        soundValue.text = "(" + soundSlider.value + "%)";

        // Crosshair settings
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

        if(Crosshair.sprite >= 4)
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

        // Flashing effects settings
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
    }

    public void SetMouseSensitivity()
    {
        mouseSensitivity = mouseSensitivitySlider.value / 10;
        SaveOptions();
    }

    public void SetMusicVolume()
    {
        musicVolume = musicSlider.value / 100;
        SaveOptions();
    }

    public void SetSoundVolume()
    {
        soundVolume = soundSlider.value / 100;
        SaveOptions();
    }

    public void ChangeCrosshair(int increment)
    {
        Crosshair.sprite += increment;
        SaveOptions();
    }

    public void ChangeFlashingFX(int increment)
    {
        flashingEffects += increment;
        SaveOptions();
    }

    public static void SaveOptions()
    {
        PlayerPrefs.SetFloat("global_mouse_sensitivity", mouseSensitivity);
        PlayerPrefs.SetFloat("global_music_volume", musicVolume);
        PlayerPrefs.SetFloat("global_sfx_volume", soundVolume);
        PlayerPrefs.SetInt("global_crosshair", Crosshair.sprite);
        PlayerPrefs.SetInt("global_flashing", flashingEffects);
    }

    public static void ResetOptions()
    {
        mouseSensitivity = 1.0f;
        musicVolume = 0.5f;
        soundVolume = 1.0f;
        Crosshair.sprite = 0;
        flashingEffects = 0;
        SaveOptions();
        PlayerPrefs.Save();

        Debug.Log("Reset options.");
    }
}
