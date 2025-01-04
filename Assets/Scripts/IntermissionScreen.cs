using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntermissionScreen : MonoBehaviour
{
    // Intermission screen script. This script does not change the currentChapter and currentMap variables. Only the MapProperties script does that.

    #region Variables

    public GameObject fadeFrom;
    public GameObject fadeTo;

    public Text score;
    public Text scoreTotal;
    public Text secrets;
    public Text secretsRating;
    public Text enemies;
    public Text enemiesRating;
    public Text time;
    public Text continueText;
    public AudioSource bonusSound;

    private bool _secretsBonus = false;
    private bool _enemiesBonus = false;

    private int _intermissionState = 0;
    private int _displayNumber = 0;
    private int _displayNumber2 = 0;

    private AudioSource _as;
    private bool _endedIntermission = false;

    #endregion

    #region Default Methods

    void Start()
    {
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Enemy.sightSoundsPlaying = 0;
        _as = GetComponent<AudioSource>();

        Instantiate(fadeFrom, gameObject.transform);

        _intermissionState = 0;
        _endedIntermission = false;

        secretsRating.enabled = false;
        enemiesRating.enabled = false;

        if (StaticClass.intermissionDisplayType == 0)
        {
            score.text = "SCORE: " + Player.scoreThisLevel.ToString();
            secrets.text = "SECRETS: " + StaticClass.secretsDiscovered.ToString() + " / " + StaticClass.secretsTotal.ToString();
            enemies.text = "FOES: " + StaticClass.enemiesKilled.ToString() + " / " + StaticClass.enemiesTotal.ToString();
            scoreTotal.text = "TOTAL SCORE: " + Player.score.ToString();
            ShowTimeText(Player.timeMinutes, Player.timeSeconds);
            _intermissionState = 5;
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

        // If this is the latest unlocked chapter, and the last map in the current chapter, unlock the next chapter as soon as the intermission screen starts.
        if (StaticClass.currentChapter == StaticClass.unlockedChapter && StaticClass.currentMap >= 5)
        {
            StaticClass.unlockedChapter++;

            if (Debug.isDebugBuild == true)
            {
                Debug.Log("Unlocked new chapter!");
            }
        }

        SaveChapterHighScore();
    }

    void Update()
    {
        if (StaticClass.intermissionDisplayType == 1)
        {
            if (_intermissionState > 1)
            {
                ShowScoreText(Player.scoreThisLevel);
                ShowScoreTotalText(Player.score);
            }
            if (_intermissionState > 2)
            {
                ShowSecretText(StaticClass.secretsDiscovered, false);
            }
            if (_intermissionState > 3)
            {
                ShowEnemyText(StaticClass.enemiesKilled, false);
            }
            if (_intermissionState > 4)
            {
                ShowTimeText(Player.timeMinutes, Player.timeSeconds);
            }
            if (_intermissionState >= 5)
            {
                continueText.enabled = true;
            }
            else
            {
                continueText.enabled = false;
            }
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Time.timeSinceLevelLoad >= 1)
        {
            if (_intermissionState >= 5)
            {
                if (_endedIntermission == false)
                {
                    StartCoroutine(EndIntermission());
                }
            }
            else
            {
                NextIntermissionState();
            }
        }
    }

    #endregion

    #region Info Display

    IEnumerator GradualDisplay()
    {
        yield return new WaitForSeconds(0.04f);

        if (_intermissionState == 0)
        {
            NextIntermissionState();
        }
        else if (_intermissionState == 1)
        {
            // Level score.
            ShowScoreText(_displayNumber);
            ShowScoreTotalText((Player.score - Player.scoreThisLevel) + _displayNumber2);

            if (_displayNumber >= Player.scoreThisLevel)
            {
                NextIntermissionState();
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                _displayNumber += 655;
                _displayNumber2 += 655;
            }
        }
        else if (_intermissionState == 2)
        {
            // Secrets.
            ShowSecretText(_displayNumber, true);

            if (_displayNumber >= StaticClass.secretsDiscovered)
            {
                NextIntermissionState();
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                _displayNumber += 1;
            }
        }
        else if (_intermissionState == 3)
        {
            // Enemies killed.
            ShowEnemyText(_displayNumber, true);

            if (_displayNumber >= StaticClass.enemiesKilled)
            {
                NextIntermissionState();
                yield return new WaitForSeconds(0.75f);
            }
            else
            {
                _displayNumber += 1;
            }
        }
        else if (_intermissionState == 4)
        {
            // Time taken.
            ShowTimeText(_displayNumber2, _displayNumber);

            if (_displayNumber > 59)
            {
                _displayNumber2++;
                _displayNumber = 0;
            }

            if (_displayNumber + (_displayNumber2 * 60) >= Player.timeSeconds + (Player.timeMinutes * 60))
            {
                NextIntermissionState();
            }
            else
            {
                // Time ticking animation. Adds in greater increments the longer the player took to complete the level.

                // Add seconds.
                if (Player.timeMinutes < 1)
                {
                    _displayNumber += 1;
                }
                else if (Player.timeMinutes < 10)
                {
                    _displayNumber += 3;
                }
                else if (Player.timeMinutes < 20)
                {
                    _displayNumber += 6;
                }
                else if (Player.timeMinutes < 30)
                {
                    _displayNumber += 12;
                }
                else if (Player.timeMinutes < 40)
                {
                    _displayNumber += 24;
                }
                else
                {
                    _displayNumber += 48;
                }

                // Add minutes.
                if (Player.timeMinutes >= 300)
                {
                    _displayNumber2 += 1;
                }
            }
        }

        PlayTickSound();

        if (_intermissionState < 5)
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

            if (_secretsBonus == false)
            {
                _secretsBonus = true;
                Player.score += 8000;
                PlayBonusSound();
                SaveChapterHighScore();
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

            if (_enemiesBonus == false)
            {
                _enemiesBonus = true;
                Player.score += 8000;
                PlayBonusSound();
                SaveChapterHighScore();
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
        _displayNumber = 0;
        _displayNumber2 = 0;
        _intermissionState++;
    }

    #endregion

    #region Sounds

    void PlayTickSound()
    {
        _as.Stop();
        _as.Play();
    }

    void PlayBonusSound()
    {
        bonusSound.Play();
    }

    #endregion

    #region Navigation

    IEnumerator EndIntermission()
    {
        _endedIntermission = true;
        _intermissionState = 5;
        SaveChapterHighScore();
        SaveSystem.SaveGlobal();

        // Create fade in object.
        Instantiate(fadeTo, gameObject.transform);

        yield return new WaitForSeconds(1.85f);

        // Reset variables. Doesn't reset chapter and map variables.
        StaticClass.ResetStats(false);

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

    void SaveChapterHighScore()
    {
        // Save chapter high score.
        if (SaveSystem.GetSavedGlobal() == null)
        {
            // Has no saved global stats.
            StaticClass.chapterHighScore[StaticClass.currentChapter - 1] = Player.score;
        }
        else if (SaveSystem.GetSavedGlobal().chapterHighScore[StaticClass.currentChapter - 1] < Player.score)
        {
            // Already has a save file for global stats. Replace old high score if current score is higher.
            StaticClass.chapterHighScore[StaticClass.currentChapter - 1] = Player.score;
        }

        SaveSystem.SaveGlobal();
    }

    #endregion
}
