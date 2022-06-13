using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyHUD : MonoBehaviour
{
    Image img;
    Player playerScript;
    public int keyId = 0;

    void Start()
    {
        img = GetComponent<Image>();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if(playerScript.keys[keyId] == true)
        {
            img.enabled = true;
        }
        else
        {
            img.enabled = false;
        }
    }
}
