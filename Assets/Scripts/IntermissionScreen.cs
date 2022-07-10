using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntermissionScreen : MonoBehaviour
{
    public GameObject fadeFrom;
    public Text score;
    public Text secrets;
    public Text enemies;
    public Text time;

    void Start()
    {
        Instantiate(fadeFrom, gameObject.transform);
        score.text = "SCORE: " + Player.score.ToString();
        secrets.text = "SECRETS: " + StaticClass.secretsDiscovered.ToString() + " / " + StaticClass.secretsTotal.ToString();
        enemies.text = "FOES: " + StaticClass.enemiesKilled.ToString() + " / " + StaticClass.enemiesTotal.ToString();
        time.text = "TIME: " + Player.timeMinutes.ToString() + " : ";
        
        if(Player.timeSeconds > 9)
        {
            time.text += Player.timeSeconds.ToString();
        }
        else
        {
            time.text += "0" + Player.timeSeconds.ToString();
        }

        // Reset variables after displaying text

        StaticClass.secretsDiscovered = 0;
        StaticClass.secretsTotal = 0;
        StaticClass.enemiesTotal = 0;
        Player.timeSeconds = 0;
        Player.timeMinutes = 0;

        // Completed a chapter. Each chapter has 5 maps

        if(StaticClass.mapReadOnly >= 5)
        {
            StaticClass.chapterReadOnly++;
            StaticClass.mapReadOnly = 0;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("C" + StaticClass.chapterReadOnly + "M" + (StaticClass.mapReadOnly + 1));
        }
    }
}
