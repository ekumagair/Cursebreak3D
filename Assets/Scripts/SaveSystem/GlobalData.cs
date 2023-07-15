using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalData
{
    public float mouseSensitivity;
    public float musicVolume;
    public float soundVolume;
    public int crosshairSprite;
    public int flashingEffects;

    public int unlockedChapters;
    public int[] chapterHighScore = new int[3];

    public GlobalData()
    {
        mouseSensitivity = Options.mouseSensitivity;
        musicVolume = Options.musicVolume;
        soundVolume = Options.soundVolume;
        crosshairSprite = Crosshair.sprite;
        flashingEffects = Options.flashingEffects;

        unlockedChapters = StaticClass.unlockedChapter;

        for (int i = 0; i < chapterHighScore.Length; i++)
        {
            chapterHighScore[i] = StaticClass.chapterHighScore[i];
        }
    }
}
