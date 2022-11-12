using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public Text difficultyText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        StaticClass.loadSavedPlayerInfo = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            StaticClass.difficulty--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            StaticClass.difficulty++;
        }
        difficultyText.text = "Difficulty: " + StaticClass.difficulty.ToString();
    }

    public void GoToScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
