using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogMessageScript : MonoBehaviour
{
    #region Variables

    private HUD _hudScript;
    private RectTransform _rt;
    private Text _txt;

    #endregion

    #region Default Methods

    void Start()
    {
        _txt = GetComponent<Text>();
        _hudScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
        StartCoroutine(RemoveThis());
    }

    void Update()
    {
        if (Time.timeScale == 0.0f)
        {
            _txt.enabled = false;
        }
        else
        {
            _txt.enabled = true;
        }
    }

    #endregion

    #region Message

    public void MoveUp()
    {
        _rt = GetComponent<RectTransform>();
        _rt.anchoredPosition += new Vector2(0, _rt.rect.height);
    }

    private IEnumerator RemoveThis()
    {
        yield return new WaitForSeconds(3f);
        _hudScript.HudMoveUpLog();

        Destroy(gameObject);
    }

    #endregion
}
