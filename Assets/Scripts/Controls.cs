using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    #region Variables

    [Header("Physics")]
    public float vel = 12f;
    public float velSprintMult = 1.6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public bool canJump = true;
    public KeyCode sprintKeyCode;
    public bool isSprinting = false;
    Vector3 _velocityV3;
    Vector3 _recordedPosition = Vector3.zero;

    [Header("Collision")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask solidMask;
    public LayerMask useMask;

    [Header("Checks")]
    public bool isGrounded;
    public bool isInputtingMovement;
    public bool isChangingPosition;

    [Header("Footstep Sounds")]
    public AudioClip[] steps;
    public bool hasWalkStepSFX = true;
    public bool hasSprintStepSFX = true;
    private AudioSource _audioSource;

    [Header("Use")]
    public KeyCode useKey;
    public AudioClip cantUse;

    private CharacterController _controller;
    private Camera _mainCam;
    private Player _playerScript;
    private Health _healthScript;

    #endregion

    #region Default Methods

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _controller = GetComponent<CharacterController>();
        _mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _playerScript = GetComponent<Player>();
        _healthScript = GetComponent<Health>();
        isSprinting = false;

        StartCoroutine(Footstep());
    }

    void Update()
    {
        // Movement
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, solidMask);

        if (isGrounded && _velocityV3.y < 0)
        {
            _velocityV3.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0 || z != 0)
        {
            isInputtingMovement = true;
        }
        else
        {
            isInputtingMovement = false;
        }

        Vector3 move = transform.right * x + transform.forward * z;

        // Sprint
        if (Input.GetKey(sprintKeyCode) && _playerScript.conditionTimer[2] <= 0)
        {
            move *= velSprintMult;
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        isChangingPosition = _recordedPosition != transform.position;
        _recordedPosition = transform.position;

        // Execute horizontal movement.
        if (_controller.enabled == true && HUD.minimapEnabled == false && _playerScript.conditionTimer[1] <= 0)
        {
            _controller.Move(move * vel * Time.deltaTime);
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded && canJump && StaticClass.gameState == 0)
        {
            _velocityV3.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            FootstepSFX();
        }

        _velocityV3.y += gravity * Time.deltaTime;

        if (_controller.enabled == true)
        {
            // Execute vertical movement.
            _controller.Move(_velocityV3 * Time.deltaTime);
        }

        // Use
        if (Input.GetKeyDown(useKey) && StaticClass.gameState == 0 && Time.timeScale > 0.0f)
        {
            if (StaticClass.debugRays == true)
            {
                Debug.DrawRay(transform.position, _mainCam.transform.forward * 4, Color.white, 5f);
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, _mainCam.transform.forward, out hit, 4f, useMask))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<Door>() != null)
                    {
                        /*
                        if (Debug.isDebugBuild == true)
                        {
                            Debug.Log("Used door");
                        }*/

                        Door doorScript = hit.collider.gameObject.GetComponent<Door>();

                        if (doorScript.doorState == 0 && doorScript.canUse == true)
                        {
                            StartCoroutine(doorScript.OpenDoor());

                            if (_playerScript.keys[doorScript.key] == false)
                            {
                                if (doorScript.key == 1)
                                {
                                    HUD.Instance.HudMessage("You need a bronze key to open this door", 3f);
                                }
                                else if (doorScript.key == 2)
                                {
                                    HUD.Instance.HudMessage("You need a silver key to open this door", 3f);
                                }
                                else if (doorScript.key == 3)
                                {
                                    HUD.Instance.HudMessage("You need a golden key to open this door", 3f);
                                }
                            }
                        }
                    }
                    if (hit.collider.gameObject.GetComponent<MovingWall>() != null)
                    {
                        /*
                        if (Debug.isDebugBuild == true)
                        {
                            Debug.Log("Used moving wall");
                        }*/

                        MovingWall wallScript = hit.collider.gameObject.GetComponent<MovingWall>();

                        if (wallScript.wallState == 0)
                        {
                            StartCoroutine(wallScript.MoveWall());
                        }
                    }
                    if (hit.collider.gameObject.GetComponent<Exit>() != null)
                    {
                        /*
                        if (Debug.isDebugBuild == true)
                        {
                            Debug.Log("Used exit");
                        }*/

                        Exit exitScript = hit.collider.gameObject.GetComponent<Exit>();

                        exitScript.UsedExit();
                        StartCoroutine(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Exit(exitScript.fade));
                    }
                    if (hit.collider.gameObject.name == "HeartDoor")
                    {
                        HUD.Instance.HudMessage("You need a heart to open this door", 3f);
                    }
                }
                else
                {
                    //Debug.Log("Used");
                }
            }
            else
            {
                //Debug.Log("Can't use");
                _audioSource.PlayOneShot(cantUse);
            }
        }
    }

    #endregion

    #region Footsteps

    private IEnumerator Footstep()
    {
        yield return new WaitForSeconds(4.5f / GetCurrentVelocity());

        if (isInputtingMovement && isGrounded && isChangingPosition && HUD.minimapEnabled == false)
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

    private void FootstepSFX()
    {
        if (_healthScript.health > 0 && StaticClass.gameState == 0)
        {
            if (!isSprinting && hasWalkStepSFX)
            {
                _audioSource.PlayOneShot(steps[Random.Range(0, steps.Length)], 0.5f);
            }
            if (isSprinting && hasSprintStepSFX)
            {
                _audioSource.PlayOneShot(steps[Random.Range(0, steps.Length)], 0.7f);
            }
        }
    }

    #endregion

    #region Checks

    private float GetCurrentVelocity()
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

    #endregion
}
