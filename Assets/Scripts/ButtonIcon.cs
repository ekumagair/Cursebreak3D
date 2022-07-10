using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonIcon : MonoBehaviour
{
    public void IconPos(Transform t)
    {
        GetComponent<RectTransform>().position = new Vector3 (t.position.x - 220, t.position.y, t.position.z);
    }
}
