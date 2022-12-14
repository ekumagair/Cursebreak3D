using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryScreen : MonoBehaviour
{
    public Text storyText;

    public static int whichText = 0;
    public static bool goToTitle = false;

    void Start()
    {
        switch (whichText)
        {
            default:
                storyText.text = "";
                break;
            case 0:
                storyText.text = "As a scavenger, you are constantly on the move. The sparsely populated grass fields are the best way for you to move from town to town.\n\nAlthough they are home to some monsters, crossing them once again shouldn't be a problem.";
                break;
            case 1:
                storyText.text = "The fortress is a lot quieter than you expected. The entrance gates are opened. You were hoping to find help, yet this place seems abandoned.\n\nHowever, turning back means you'll have to face the infestation of creatures by yourself. With little choice, you enter the building, now unsure of what to expect inside.";
                break;
            case 2:
                storyText.text = "At the entrance to the underground, the monsters' presence feels even stronger.\n\nThe source of the infestation could be down there, and though the risk is great, eliminating it could bring an end to this.\n\nYou've reached this far, no reason to stop now.";
                break;
            case 3:
                storyText.text = "Chapter 1 End.";
                break;
            case 4:
                storyText.text = "Chapter 2 End.";
                break;
            case 5:
                storyText.text = "Chapter 3 End.";
                break;
        }

        storyText.text += "\n\n(Press [Space] or [Enter] to continue)";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (goToTitle == false)
            {
                SceneManager.LoadScene("C" + StaticClass.currentChapter + "M" + StaticClass.currentMap);
            }
            else
            {
                SceneManager.LoadScene("TitleScreen");
            }
        }
    }
}
