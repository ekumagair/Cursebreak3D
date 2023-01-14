using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryScreen : MonoBehaviour
{
    public GameObject fadeFrom;
    public GameObject fadeTo;

    public Text storyText;

    public static int whichText = 0;
    public static bool goToTitle = false;

    AudioSource _as;
    bool continued = false;

    void Start()
    {
        Time.timeScale = 1.0f;
        Instantiate(fadeFrom, gameObject.transform);

        _as = GetComponent<AudioSource>();
        _as.mute = false;
        continued = false;

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
                storyText.text = "At the entrance to the newly-opened underground section, the monsters' presence feels even stronger.\n\nThe source of the infestation could be down there, and though the risk is great, eliminating it could bring an end to this.\n\nYou've reached this far, no reason to stop now.";
                break;
            case 3:
                storyText.text = "You encountered a lot more monsters than you expected. They were also much more powerful than you are used to seeing.\n\nThere has to be something causing this infestation, and you don't want to face it alone. There is a fortress nearby. Perhaps its inhabitants could help you.";
                break;
            case 4:
                storyText.text = "The fortress had nobody that could help you. On the contrary, it has also been taken over by the monsters.\n\nFortunately, the situation made you improve your combat skills, because the key you just received unlocks a new part of the fortress. One that seems a lot more dangerous.";
                break;
            case 5:
                storyText.text = "The Summoner has been defeated. The monster infestation is now contained. This world already has enough evil creatures as it is.\n\nAfter you close the last cursed spell book, you feel relieved, knowing that your unexpected quest has ended in victory. Congratulations!";
                break;
        }

        storyText.text += "\n\n(Press [Space] or [Enter] to continue)";
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && continued == false)
        {
            StartCoroutine(Continue());
        }
    }

    IEnumerator Continue()
    {
        continued = true;

        Instantiate(fadeTo, gameObject.transform);

        yield return new WaitForSeconds(1.45f);

        _as.mute = true;

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
