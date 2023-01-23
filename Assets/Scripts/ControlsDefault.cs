using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsDefault : MonoBehaviour
{
    CharacterController controller;
    Camera mainCam;

    [Header("Physics")]
    public float vel = 12f;
    public float velSprintMult = 1.8f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public bool canJump = true;
    public KeyCode sprintKeyCode;
    public bool isSprinting = false;

    [Header("Collision")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask solidMask;
    public LayerMask useMask;
    Vector3 velocityV3;

    [Header("Checks")]
    public bool isGrounded;
    public bool isMoving;

    [Header("Footstep Sounds")]
    public AudioClip[] steps;
    public bool hasWalkStepSFX = true;
    public bool hasSprintStepSFX = true;
    AudioSource audioSource;

    [Header("Use")]
    public KeyCode useKey;
    public AudioClip cantUse;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        isSprinting = false;
        StartCoroutine(Footstep());
    }

    void Update()
    {
        // Movement
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, solidMask);

        if (isGrounded && velocityV3.y < 0)
        {
            velocityV3.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint
        if (Input.GetKey(sprintKeyCode))
        {
            move *= velSprintMult;
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if (controller.enabled == true)
        {
            controller.Move(move * vel * Time.deltaTime);
        }

        // Jump

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocityV3.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            FootstepSFX();
        }

        velocityV3.y += gravity * Time.deltaTime;

        if (controller.enabled == true)
        {
            controller.Move(velocityV3 * Time.deltaTime);
        }

        // Use
        if (Input.GetKeyDown(useKey))
        {
            Debug.DrawRay(transform.position, mainCam.transform.forward * 4, Color.white, 5f);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, mainCam.transform.forward, out hit, 4f, useMask))
            {
                if (hit.collider != null)
                {
                    
                }
                else
                {
                    Debug.Log("Used");
                }
            }
            else
            {
                Debug.Log("Can't use");
                audioSource.PlayOneShot(cantUse);
            }
        }
    }

    IEnumerator Footstep()
    {
        yield return new WaitForSeconds(3.6f / GetCurrentVelocity());

        if (isMoving && isGrounded)
        {
            FootstepSFX();
        }

        StartCoroutine(Footstep());
    }

    float GetCurrentVelocity()
    {
        if (!isSprinting)
        {
            return vel;
        }
        else
        {
            return vel * velSprintMult;
        }
    }

    void FootstepSFX()
    {
        if (!isSprinting && hasWalkStepSFX)
        {
            audioSource.PlayOneShot(steps[Random.Range(0, steps.Length)], 0.5f);
        }
        if (isSprinting && hasSprintStepSFX)
        {
            audioSource.PlayOneShot(steps[Random.Range(0, steps.Length)], 0.7f);
        }
    }
}
