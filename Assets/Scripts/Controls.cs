using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public CharacterController controller;
    Camera mainCam;

    public float vel = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public bool canJump = true;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask solidMask;
    public LayerMask useMask;

    Vector3 velocityV3;
    public bool isGrounded;
    public bool isMoving;

    AudioSource audioSource;
    public AudioClip[] steps;
    public bool hasStepSFX = true;

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

        if(x != 0 || z != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        if (controller.enabled == true)
        {
            controller.Move(move * vel * Time.deltaTime);
        }

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
                    if (hit.collider.gameObject.GetComponent<Door>() != null)
                    {
                        Debug.Log("Used door");

                        Door doorScript = hit.collider.gameObject.GetComponent<Door>();

                        if (doorScript.doorState == 0)
                        {
                            StartCoroutine(doorScript.OpenDoor());
                        }
                    }
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

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject en in enemies)
        {
            if (en.GetComponent<Enemy>() != null)
            {
                if (en.GetComponent<Enemy>().CanSee(gameObject, 30f))
                {
                    en.GetComponent<Enemy>().target = gameObject;
                }
            }
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
