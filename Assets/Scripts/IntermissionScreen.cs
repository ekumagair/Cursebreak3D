using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntermissionScreen : MonoBehaviour
{
    public GameObject fadeFrom;
    public Text score;

    void Start()
    {
        Instantiate(fadeFrom, gameObject.transform);
        score.text = "SCORE: " + Player.score.ToString();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("C" + StaticClass.chapterReadOnly + "M" + (StaticClass.mapReadOnly + 1));
        }
    }
}
