using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public static int sprite = 0;
    public Sprite[] spriteList;

    Image img;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        img.sprite = spriteList[sprite];

        if(HUD.minimapEnabled == false && sprite > 0 && Time.timeScale > 0.0f)
        {
            img.enabled = true;
        }
        else
        {
            img.enabled = false;
        }
    }
}
