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
    }

    public void SetMouseSensitivity()
    {
        mouseSensitivity = mouseSensitivitySlider.value / 10;
        SaveSettings();
    }

    public void SetMusicVolume()
    {
        musicVolume = musicSlider.value / 100;
        SaveSettings();
    }

    public void SetSoundVolume()
    {
        soundVolume = soundSlider.value / 100;
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("global_mouse_sensitivity", mouseSensitivity);
        PlayerPrefs.SetFloat("global_music_volume", musicVolume);
        PlayerPrefs.SetFloat("global_sfx_volume", soundVolume);
    }
}
