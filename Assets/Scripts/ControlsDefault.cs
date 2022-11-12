using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsDefault : MonoBehaviour
{
    public CharacterController controller;
    Camera mainCam;

    [Header("Physics")]
    public float vel = 12f;
    public float velSprintMult = 1.8f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public bool canJump = true;
    public KeyCode sprintKeyCode;

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
    public bool hasStepSFX = true;
    AudioSource audioSource;

    [Header("Use")]
    public KeyCode useKey;
    public AudioClip cantUse;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
        }

        if (controller.enabled == true)
        {
            controller.Move(move * vel * Time.deltaTime);
        }

        // Jump

        if (Input.GetButtonDown("Jump") && isGrounded && canJump)
        {
            velocityV3.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            FootstepSFXChoose();
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
                Sound(cantUse);
            }
        }
    }

    IEnumerator Footstep()
    {
        yield return new WaitForSeconds(0.45f);

        if (isMoving && isGrounded)
        {
            FootstepSFXChoose();
        }

        StartCoroutine(Footstep());
    }

    void FootstepSFXChoose()
    {
        if (hasStepSFX)
        {
            Sound(steps[Random.Range(0, steps.Length)]);
        }
    }

    void Sound(AudioClip s)
    {
        audioSource.clip = s;

        if (s != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
