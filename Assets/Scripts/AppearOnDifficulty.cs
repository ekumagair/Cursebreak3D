using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearOnDifficulty : MonoBehaviour
{
    public bool[] difficulty = new bool[4] { true, true, true, true };

    void Start()
    {
        if (StaticClass.difficulty <= difficulty.Length)
        {
            if (difficulty[StaticClass.difficulty] == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
