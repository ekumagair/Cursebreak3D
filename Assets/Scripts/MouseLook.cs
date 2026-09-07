using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    #region Variables

    public float mouseS = 1000f;
    private float _mouseX, _mouseY;

    public Transform playerBody;
    public Player playerScript;
    public bool lookX = true, lookY = true;

    private float _xRot = 0f;

    #endregion

    #region Default Methods

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _mouseX = 0;
        _mouseY = 0;
    }

    void Update()
    {
        if (lookX && playerScript.conditionTimer[5] <= 0)
        {
            _mouseX = Input.GetAxis("Mouse X") * mouseS * Options.mouseSensitivity * Time.deltaTime;
        }
        if (lookY && playerScript.conditionTimer[6] <= 0)
        {
            _mouseY = Input.GetAxis("Mouse Y") * mouseS * Options.mouseSensitivity * Time.deltaTime;
        }

        _xRot -= _mouseY;
        _xRot = Mathf.Clamp(_xRot, -90f, 90f);

        if (HUD.minimapEnabled == false)
        {
            transform.localRotation = Quaternion.Euler(_xRot, 0f, 0f);
            playerBody.Rotate(Vector3.up * _mouseX);
        }
    }

    #endregion
}
