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

    private Button _btn;
    private EventTrigger _events;
    private Text _buttonText;

    void Start()
    {
        _btn = GetComponent<Button>();
        _events = GetComponent<EventTrigger>();
        _buttonText = GetComponentInChildren<Text>();
        _buttonText.enabled = false;
    }

    void Update()
    {
        if (fullStringOverride == "")
        {
            _buttonText.text = chapterNumber.ToString() + " - ";
        }
        else
        {
            _buttonText.text = fullStringOverride;
        }

        if(StaticClass.unlockedChapter >= chapterNumber || StaticClass.ignoreUnlockedChapter == true)
        {
            _btn.interactable = true;
            _events.enabled = true;
            _buttonText.enabled = true;

            if (fullStringOverride == "")
            {
                _buttonText.text += chapterName;
            }
        }
        else
        {
            _btn.interactable = false;
            _events.enabled = false;

            if (hideWhenLocked == false)
            {
                _buttonText.text += "(LOCKED)";
                _buttonText.enabled = true;
            }
            else
            {
                _buttonText.enabled = false;
            }
        }
    }
}
