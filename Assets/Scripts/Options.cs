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

    public static float mouseSensitivity = 1.0f;
    public static float musicVolume = 0.5f;
    public static float soundVolume = 1.0f;

    void Start()
    {
        if (PlayerPrefs.HasKey("global_mouse_sensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("global_mouse_sensitivity");
            musicVolume = PlayerPrefs.GetFloat("global_music_volume");
            soundVolume = PlayerPrefs.GetFloat("global_sfx_volume");
            Crosshair.sprite = PlayerPrefs.GetInt("global_crosshair");
        }

        mouseSensitivitySlider.value = mouseSensitivity * 10;
        musicSlider.value = musicVolume * 100;
        soundSlider.value = soundVolume * 100;
    }

    void Update()
    {
        mouseSensitivityValue.text = "(" + mouseSensitivity + "x)";
        musicValue.text = "(" + musicSlider.value + "%)";
        soundValue.text = "(" + soundSlider.value + "%)";

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

    public static void SaveOptions()
    {
        PlayerPrefs.SetFloat("global_mouse_sensitivity", mouseSensitivity);
        PlayerPrefs.SetFloat("global_music_volume", musicVolume);
        PlayerPrefs.SetFloat("global_sfx_volume", soundVolume);
        PlayerPrefs.SetInt("global_crosshair", Crosshair.sprite);
    }

    public static void ResetOptions()
    {
        mouseSensitivity = 1.0f;
        musicVolume = 0.5f;
        soundVolume = 1.0f;
        Crosshair.sprite = 0;
        SaveOptions();
        PlayerPrefs.Save();

        Debug.Log("Reset options.");
    }
}
