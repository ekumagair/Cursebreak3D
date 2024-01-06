using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalData
{
    public string gameVersion;

    public float mouseSensitivity;
    public float musicVolume;
    public float soundVolume;
    public int crosshairSprite;
    public int flashingEffects;
    public int gameResolution;

    public int unlockedChapters;
    public int[] chapterHighScore = new int[3];

    public GlobalData()
    {
        gameVersion = Application.version.ToString();

        mouseSensitivity = Options.mouseSensitivity;
        musicVolume = Options.musicVolume;
        soundVolume = Options.soundVolume;
        crosshairSprite = Crosshair.sprite;
        flashingEffects = Options.flashingEffects;
        gameResolution = Options.gameResolution;

        unlockedChapters = StaticClass.unlockedChapter;

        for (int i = 0; i < chapterHighScore.Length; i++)
        {
            chapterHighScore[i] = StaticClass.chapterHighScore[i];
        }
    }
}
