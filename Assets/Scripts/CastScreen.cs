using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CastScreen : MonoBehaviour
{
    #region Variables

    public GameObject characterDisplay;
    public Text characterName;
    public Text endText;

    public string[] characterNameList;

    private Animator _characterDisplayAnimator;
    private Image _characterDisplayImage;
    private int _indexValue = 0;

    #endregion

    #region Default Methods

    void Start()
    {
        Time.timeScale = 1.0f;
        StaticClass.ResetStats(false);
        Cursor.lockState = CursorLockMode.None;

        _characterDisplayAnimator = characterDisplay.GetComponent<Animator>();
        _characterDisplayImage = characterDisplay.GetComponent<Image>();
        _indexValue = 0;
    }

    void Update()
    {
        characterName.text = characterNameList[_indexValue];

        if (characterName.text == "")
        {
            _characterDisplayImage.enabled = false;
            endText.enabled = true;
        }
        else
        {
            _characterDisplayImage.enabled = true;
            endText.enabled = false;
        }

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Time.timeSinceLevelLoad >= 1)
        {
            _characterDisplayAnimator.SetTrigger("Next");

            if (_indexValue < characterNameList.Length - 1)
            {
                _indexValue++;
            }
            else
            {
                _indexValue = 0;
                _characterDisplayAnimator.ResetTrigger("Next");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }

    #endregion
}
