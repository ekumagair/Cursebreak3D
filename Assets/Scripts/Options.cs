using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider mouseSensitivitySlider;
    public Slider musicSlider;
    public Slider soundSlider;

    public static float mouseSensitivity = 1.0f;
    public static float musicVolume = 0.5f;
    public static float soundVolume = 1.0f;

    void Start()
    {
        mouseSensitivitySlider.value = mouseSensitivity;
        musicSlider.value = musicVolume;
        soundSlider.value = soundVolume;
    }

    public void SetMouseSensitivity()
    {
        mouseSensitivity = mouseSensitivitySlider.value;
        SaveSettings();
    }

    public void SetMusicVolume()
    {
        musicVolume = musicSlider.value;
        SaveSettings();
    }

    public void SetSoundVolume()
    {
        soundVolume = soundSlider.value;
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("global_mouse_sensitivity", mouseSensitivity);
        PlayerPrefs.SetFloat("global_music_volume", musicVolume);
        PlayerPrefs.SetFloat("global_sfx_volume", soundVolume);
    }
}
