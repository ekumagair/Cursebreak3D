using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    #region Variables

    public static int sprite = 0;
    public Sprite[] spriteList;

    private Image _img;

    #endregion

    #region Default Methods

    void Start()
    {
        _img = GetComponent<Image>();
    }

    void Update()
    {
        _img.sprite = spriteList[sprite];

        if (HUD.minimapEnabled == false && sprite > 0 && Time.timeScale > 0.0f)
        {
            _img.enabled = true;
        }
        else
        {
            _img.enabled = false;
        }
    }

    #endregion
}
