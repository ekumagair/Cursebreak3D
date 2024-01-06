using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSectionSelectLevel : MonoBehaviour
{
    public Text chapterText;
    public Button chapterAdd;
    public Button chapterSubtract;

    public Text mapText;
    public Button mapAdd;
    public Button mapSubtract;

    public static bool levelWarp = false;

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
}
