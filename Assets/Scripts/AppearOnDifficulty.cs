using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearOnDifficulty : MonoBehaviour
{
    public bool[] difficulty = new bool[4] { true, true, true, true };

    void Start()
    {
        // Don't do anything to this object if the difficulty value is outside the normal range (greater than the array length or smaller than zero).
        if (StaticClass.difficulty < difficulty.Length && StaticClass.difficulty > -1)
        {
            // Destroy this object if it's not meant to appear in this difficulty value.
            if (difficulty[StaticClass.difficulty] == false)
            {
                Destroy(gameObject);
            }
        }
    }
}
