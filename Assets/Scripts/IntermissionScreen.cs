using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntermissionScreen : MonoBehaviour
{
    public GameObject fadeFrom;
    public Text score;
    public Text scoreTotal;
    public Text secrets;
    public Text secretsRating;
    public Text enemies;
    public Text enemiesRating;
    public Text time;
    public AudioSource bonusSound;

    bool secretsBonus = false;
    bool enemiesBonus = false;

    int intermissionState = 0;
    int displayNumber = 0;
    int displayNumber2 = 0;

    AudioSource _as;

    void Start()
    {
        Time.timeScale = 1.0f;
        _as = GetComponent<AudioSource>();

        Instantiate(fadeFrom, gameObject.transform);

        intermissionState = 0;

        secretsRating.enabled = false;
        enemiesRating.enabled = false;

        if (StaticClass.intermissionDisplayType == 0)
        {
            score.text = "SCORE: " + Player.scoreThisLevel.ToString();
            secrets.text = "SECRETS: " + StaticClass.secretsDiscovered.ToString() + " / " + StaticClass.secretsTotal.ToString();
            enemies.text = "FOES: " + StaticClass.enemiesKilled.ToString() + " / " + StaticClass.enemiesTotal.ToString();
            scoreTotal.text = "TOTAL SCORE: " + Player.score.ToString();
            ShowTimeText(Player.timeMinutes, Player.timeSeconds);
            intermissionState = 5;
        }
        else if (StaticClass.intermissionDisplayType == 1)
        {
            ShowScoreText(0);
            ShowSecretText(0, true);
            ShowEnemyText(0, true);
            ShowScoreTotalText(Player.score - Player.scoreThisLevel);
            ShowTimeText(0, 0);
            StartCoroutine(GradualDisplay());
        }
    }

    void Update()
    {
        if (StaticClass.intermissionDisplayType == 1)
        {
            if (intermissionState > 1)
            {
                ShowScoreText(Player.scoreThisLevel);
                ShowScoreTotalText(Player.score);
            }
            if (intermissionState > 2)
            {
                ShowSecretText(StaticClass.secretsDiscovered, false);
            }
            if (intermissionState > 3)
            {
                ShowEnemyText(StaticClass.enemiesKilled, false);
            }
            if (intermissionState > 4)
            {
                ShowTimeText(Player.timeMinutes, Player.timeSeconds);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (intermissionState >= 5)
            {
                // Reset variables
                Player.scoreThisLevel = 0;
                StaticClass.secretsDiscovered = 0;
                StaticClass.secretsTotal = 0;
                StaticClass.enemiesTotal = 0;
                Player.timeSeconds = 0;
                Player.timeMinutes = 0;

                // Continue to next map or story screen
                if (StaticClass.currentMap < 5)
                {
                    SceneManager.LoadScene("C" + StaticClass.currentChapter + "M" + (StaticClass.currentMap + 1));
                }
                else
                {
                    switch (StaticClass.currentChapter)
                    {
                        default:
                            StoryScreen.whichText = 0;
                            break;
                        case 1:
                            StoryScreen.whichText = 3;
                            break;
                        case 2:
                            StoryScreen.whichText = 4;
                            break;
                        case 3:
                            StoryScreen.whichText = 5;
                            break;
                    }
                    StaticClass.loadSavedPlayerInfo = false;
                    StoryScreen.goToTitle = true;
                    SceneManager.LoadScene("Story");
                }
            }
            else
            {
                NextIntermissionState();
            }
        }
    }

    IEnumerator GradualDisplay()
    {
        yield return new WaitForSeconds(0.05f);

        if (intermissionState == 0)
        {
            NextIntermissionState();
        }
        else if (intermissionState == 1)
        {
            ShowScoreText(displayNumber);
            ShowScoreTotalText((Player.score - Player.scoreThisLevel) + displayNumber2);

            if (displayNumber > Player.scoreThisLevel)
            {
                NextIntermissionState();
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                displayNumber += 655;
                displayNumber2 += 655;
            }
        }
        else if (intermissionState == 2)
        {
            ShowSecretText(displayNumber, true);

            if (displayNumber > StaticClass.secretsDiscovered)
            {
                NextIntermissionState();
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                displayNumber += 1;
            }
        }
        else if (intermissionState == 3)
        {
            ShowEnemyText(displayNumber, true);

            if (displayNumber > StaticClass.enemiesKilled)
            {
                NextIntermissionState();
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                displayNumber += 1;
            }
        }
        else if (intermissionState == 4)
        {
            ShowTimeText(displayNumber2, displayNumber);

            if(displayNumber > 59)
            {
                displayNumber2++;
                displayNumber = 0;
            }

            if(displayNumber == Player.timeSeconds && displayNumber2 >= Player.timeMinutes)
            {
                NextIntermissionState();
            }
            else
            {
                displayNumber += 1;
            }
        }

        PlayTickSound();

        if(intermissionState < 5)
        {
            StartCoroutine(GradualDisplay());
        }
    }

    void ShowScoreText(int value)
    {
        score.text = "POINTS: " + value.ToString();
    }

    void ShowScoreTotalText(int value)
    {
        scoreTotal.text = "TOTAL SCORE: " + value.ToString();
    }

    void ShowSecretText(int discovered, bool ignoreBonus)
    {
        secrets.text = "SECRETS: " + discovered + " / " + StaticClass.secretsTotal.ToString();
        if (discovered >= StaticClass.secretsTotal && discovered > 0 && StaticClass.secretsTotal > 0 && ignoreBonus == false)
        {
            secretsRating.enabled = true;

            if(secretsBonus == false)
            {
                secretsBonus = true;
                Player.score += 5000;
                PlayBonusSound();
            }
        }
        else
        {
            secretsRating.enabled = false;
        }
    }

    void ShowEnemyText(int killed, bool ignoreBonus)
    {
        enemies.text = "FOES: " + killed + " / " + StaticClass.enemiesTotal.ToString();
        if (killed >= StaticClass.enemiesTotal && killed > 0 && StaticClass.enemiesTotal > 0 && ignoreBonus == false)
        {
            enemiesRating.enabled = true;

            if(enemiesBonus == false)
            {
                enemiesBonus = true;
                Player.score += 5000;
                PlayBonusSound();
            }
        }
        else
        {
            enemiesRating.enabled = false;
        }
    }

    void ShowTimeText(int minutes, int seconds)
    {
        time.text = "TIME: " + minutes.ToString() + " : ";

        if (seconds > 9)
        {
            time.text += seconds.ToString();
        }
        else
        {
            time.text += "0" + seconds.ToString();
        }
    }

    void NextIntermissionState()
    {
        displayNumber = 0;
        displayNumber2 = 0;
        intermissionState++;
    }

    void PlayTickSound()
    {
        _as.Stop();
        _as.Play();
    }

    void PlayBonusSound()
    {
        bonusSound.Play();
    }
}
