using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonIcon : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void IconPos(RectTransform t)
    {
        _rect.SetParent(t, false);
        _rect.anchoredPosition = new Vector3((t.sizeDelta.x * -0.5f) - (_rect.sizeDelta.x * 1.5f), 1, 0);
    }
}
