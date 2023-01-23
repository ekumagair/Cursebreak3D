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
    }

    void Update()
    {
        string key = "global_c" + selectedChapter.ToString() + "_high_score";

        if (PlayerPrefs.HasKey(key) && selectedChapter > 0)
        {
            txt.text = "Chapter " + selectedChapter.ToString() + " high score: " + PlayerPrefs.GetInt(key).ToString();
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
