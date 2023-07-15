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
        // Old player pref system.
        /*
        string key = "global_c" + selectedChapter.ToString() + "_high_score";

        if (PlayerPrefs.HasKey(key) && selectedChapter > 0)
        {
            txt.text = "Chapter " + selectedChapter.ToString() + " high score: " + PlayerPrefs.GetInt(key).ToString();
        }
        else
        {
            txt.text = "";
        }*/

        // New full serialization.
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
