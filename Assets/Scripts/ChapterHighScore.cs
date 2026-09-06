using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterHighScore : MonoBehaviour
{
    public int selectedChapter = 0;

    Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
        txt.text = "";
    }

    void Update()
    {
        if (SaveSystem.GetSavedGlobal() != null && selectedChapter > 0)
        {
            // Get the saved global data as "selectedChapter - 1" because the array starts with index 0 and valid chapters start at 1.
            txt.text = "Chapter " + selectedChapter.ToString() + " high score: " + SaveSystem.GetSavedGlobal().chapterHighScore[selectedChapter - 1];
        }
        else
        {
            txt.text = "";
        }
    }

    public void SetSelectedChapter(int value)
    {
        selectedChapter = value;
    }
}
