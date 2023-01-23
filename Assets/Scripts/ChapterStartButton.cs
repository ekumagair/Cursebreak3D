using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterStartButton : MonoBehaviour
{
    public int chapterNumber = 1;
    public string chapterName = "";
    public string fullStringOverride = "";
    public bool hideWhenLocked = false;

    Button btn;
    EventTrigger events;
    Text buttonText;

    void Start()
    {
        btn = GetComponent<Button>();
        events = GetComponent<EventTrigger>();
        buttonText = GetComponentInChildren<Text>();
        buttonText.enabled = false;
    }

    void Update()
    {
        if (fullStringOverride == "")
        {
            buttonText.text = chapterNumber.ToString() + " - ";
        }
        else
        {
            buttonText.text = fullStringOverride;
        }

        if(StaticClass.unlockedChapter >= chapterNumber || StaticClass.ignoreUnlockedChapter == true)
        {
            btn.interactable = true;
            events.enabled = true;
            buttonText.enabled = true;

            if (fullStringOverride == "")
            {
                buttonText.text += chapterName;
            }
        }
        else
        {
            btn.interactable = false;
            events.enabled = false;

            if (hideWhenLocked == false)
            {
                buttonText.text += "(LOCKED)";
                buttonText.enabled = true;
            }
            else
            {
                buttonText.enabled = false;
            }
        }
    }
}
