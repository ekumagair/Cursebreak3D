using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CastScreen : MonoBehaviour
{
    public GameObject characterDisplay;
    public Text characterName;
    public Text endText;

    public string[] characterNameList;

    Animator characterDisplayAnimator;
    Image characterDisplayImage;
    int indexValue = 0;

    void Start()
    {
        Time.timeScale = 1.0f;
        StaticClass.ResetStats(false);
        Cursor.lockState = CursorLockMode.None;

        characterDisplayAnimator = characterDisplay.GetComponent<Animator>();
        characterDisplayImage = characterDisplay.GetComponent<Image>();
        indexValue = 0;
    }

    void Update()
    {
        characterName.text = characterNameList[indexValue];

        if(characterName.text == "")
        {
            characterDisplayImage.enabled = false;
            endText.enabled = true;
        }
        else
        {
            characterDisplayImage.enabled = true;
            endText.enabled = false;
        }

        if((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) && Time.timeSinceLevelLoad >= 1)
        {
            characterDisplayAnimator.SetTrigger("Next");

            if (indexValue < characterNameList.Length - 1)
            {
                indexValue++;
            }
            else
            {
                indexValue = 0;
                characterDisplayAnimator.ResetTrigger("Next");
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
