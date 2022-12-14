using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookDefault : MonoBehaviour
{
    public float mouseS = 1000f;
    float mouseX, mouseY;

    public Transform playerBody;
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
        if (lookX)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseS * Time.deltaTime;
        }
        if (lookY)
        {
            mouseY = Input.GetAxis("Mouse Y") * mouseS * Time.deltaTime;
        }

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
