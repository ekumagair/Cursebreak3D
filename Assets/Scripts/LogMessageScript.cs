using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogMessageScript : MonoBehaviour
{
    HUD hudScript;
    RectTransform rt;
    Text txt;

    void Start()
    {
        txt = GetComponent<Text>();
        hudScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
        StartCoroutine(RemoveThis());
    }

    void Update()
    {
        if(Time.timeScale == 0.0f)
        {
            txt.enabled = false;
        }
        else
        {
            txt.enabled = true;
        }
    }

    public void MoveUp()
    {
        rt = GetComponent<RectTransform>();
        rt.position += new Vector3(0, rt.rect.height, 0);
    }

    IEnumerator RemoveThis()
    {
        yield return new WaitForSeconds(3f);
        hudScript.HudMoveUpLog();

        Destroy(gameObject);
    }
}
