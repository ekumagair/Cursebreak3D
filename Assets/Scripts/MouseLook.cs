using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseS = 1000f;
    float mouseX, mouseY;

    public Transform playerBody;
    public Player playerScript;
    public bool lookX = true, lookY = true;

    float xRot = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mouseX = 0;
        mouseY = 0;
    }

    void Update()
    {
        if (lookX && playerScript.conditionTimer[5] <= 0)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseS * Options.mouseSensitivity * Time.deltaTime;
        }
        if (lookY && playerScript.conditionTimer[6] <= 0)
        {
            mouseY = Input.GetAxis("Mouse Y") * mouseS * Options.mouseSensitivity * Time.deltaTime;
        }

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        if (HUD.mapEnabled == false)
        {
            transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
