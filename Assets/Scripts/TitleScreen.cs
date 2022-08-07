using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Text versionText;
    public GameObject sectionStart;
    public GameObject sectionChooseChapter;
    public GameObject sectionDifficulty;

    void Start()
    {
        Player.scoreThisLevel = 0;
        StaticClass.chapterReadOnly = 1;
        StaticClass.mapReadOnly = 1;
        versionText.text = "v " + Application.version.ToString();
        SectionStart();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void SectionStart()
    {
        sectionStart.SetActive(true);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(false);
    }

    public void SectionChooseChapter()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(true);
        sectionDifficulty.SetActive(false);
    }

    public void SectionDifficulty()
    {
        sectionStart.SetActive(false);
        sectionChooseChapter.SetActive(false);
        sectionDifficulty.SetActive(true);
    }

    public void SetChapter(int c)
    {
        StaticClass.chapterReadOnly = c;
    }

    public void SetDifficulty(int d)
    {
        StaticClass.difficulty = d;
    }

    public void StartChapter()
    {
        SceneManager.LoadScene("C" + StaticClass.chapterReadOnly.ToString() + "M1");
    }
}
