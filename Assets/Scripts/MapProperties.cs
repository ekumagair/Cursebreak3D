using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapProperties : MonoBehaviour
{
    public int chapter = 1;
    public int map = 1;

    private void Start()
    {
        StaticClass.chapterReadOnly = chapter;
        StaticClass.mapReadOnly = map;
    }
}
