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
        _rect.anchoredPosition = new Vector2(t.anchoredPosition.x - (t.sizeDelta.x / 2) - _rect.sizeDelta.x - 10, t.anchoredPosition.y);
    }
}
