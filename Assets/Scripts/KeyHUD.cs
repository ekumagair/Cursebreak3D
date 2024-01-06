using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyHUD : MonoBehaviour
{
    public int keyId = 0;

    private Image _img;
    private Player _playerScript;

    void Start()
    {
        _img = GetComponent<Image>();
        _playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (_playerScript.keys[keyId] == true)
        {
            _img.enabled = true;
        }
        else
        {
            _img.enabled = false;
        }
    }
}
