using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    public CharacterController controller;
    Camera mainCam;

    [Header("Physics")]
    public float vel = 12f;
    public float velSprintMult = 1.6f;
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

    HUD hudScript;
    Player playerScript;
    Health healthScript;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        hudScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>();
        playerScript = GetComponent<Player>();
        healthScript = GetComponent<Health>();
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

        if(x != 0 || z != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint
        if (Input.GetKey(sprintKeyCode) && playerScript.conditionTimer[2] <= 0)
        {
            move *= velSprintMult;
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        // Execute movement
        if (controller.enabled == true && HUD.minimapEnabled == false && playerScript.conditionTimer[1] <= 0)
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
                    if (hit.collider.gameObject.GetComponent<Door>() != null)
                    {
                        Debug.Log("Used door");
                        Door doorScript = hit.collider.gameObject.GetComponent<Door>();

                        if (doorScript.doorState == 0 && doorScript.canUse == true)
                        {
                            StartCoroutine(doorScript.OpenDoor());

                            if (playerScript.keys[doorScript.key] == false)
                            {
                                if(doorScript.key == 1)
                                {
                                    hudScript.HudMessage("You need a bronze key to open this door", 3f);
                                }
                                else if (doorScript.key == 2)
                                {
                                    hudScript.HudMessage("You need a silver key to open this door", 3f);
                                }
                                else if (doorScript.key == 3)
                                {
                                    hudScript.HudMessage("You need a golden key to open this door", 3f);
                                }
                            }
                        }
                    }
                    if (hit.collider.gameObject.GetComponent<MovingWall>() != null)
                    {
                        Debug.Log("Used moving wall");
                        MovingWall wallScript = hit.collider.gameObject.GetComponent<MovingWall>();

                        if (wallScript.wallState == 0)
                        {
                            StartCoroutine(wallScript.MoveWall());
                        }
                    }
                    if (hit.collider.gameObject.GetComponent<Exit>() != null)
                    {
                        Debug.Log("Used exit");
                        hit.collider.gameObject.GetComponent<Exit>().UsedExit();
                        StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Exit(hit.collider.gameObject.GetComponent<Exit>().fade));
                    }
                    if(hit.collider.gameObject.name == "HeartDoor")
                    {
                        hudScript.HudMessage("You need a heart to open this door", 3f);
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
                audioSource.PlayOneShot(cantUse);
            }
        }
    }

    IEnumerator Footstep()
    {
        yield return new WaitForSeconds(4f / GetCurrentVelocity());

        if (isMoving && isGrounded && HUD.minimapEnabled == false)
        {
            FootstepSFX();

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject en in enemies)
            {
                if (en.GetComponent<Enemy>() != null)
                {
                    float hearDistance;
                    if (isSprinting)
                    {
                        hearDistance = 100f;
                    }
                    else
                    {
                        hearDistance = 8f;
                    }

                    Enemy enemyScript = en.GetComponent<Enemy>();
                    if (enemyScript.CanHear(gameObject, hearDistance))
                    {
                        enemyScript.target = gameObject;
                    }
                }
            }
        }

        StartCoroutine(Footstep());
    }

    float GetCurrentVelocity()
    {
        if(!isSprinting)
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
        if (healthScript.health > 0 && StaticClass.gameState == 0)
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
}
