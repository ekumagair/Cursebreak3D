using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            ScreenCapture.CaptureScreenshot("FPS DOS screenshot " + Application.version + " " + Random.Range(0, 10000).ToString() + ".png");
            Debug.Log("Took screenshot");
        }
    }
}
