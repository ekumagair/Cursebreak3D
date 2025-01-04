using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    #region Variables

    private Vector3 defaultPosition;
    public Vector3 openPosition;
    public int wallState;
    public GameObject soundOnMove;

    [Space]

    [Tooltip("Whether this wall counts as a secret or not. Moving walls that are necessary for progression shouldn't count as secrets.")]
    public bool isSecret = true;
    public bool canBeAddedToMinimap = true;

    private string _initialPositionToString;
    private Player _player;
    private BoxCollider _boxCollider;

    #endregion

    #region Default Methods

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _boxCollider = GetComponent<BoxCollider>();

        defaultPosition = transform.position;
        _initialPositionToString = transform.position.x.ToString() + transform.position.y.ToString() + transform.position.z.ToString();

        if (isSecret)
        {
            StaticClass.secretsTotal++;
        }

        if(StaticClass.difficulty <= -2)
        {
            InstantMove();
        }
    }

    void Update()
    {
        if (transform.position == defaultPosition)
        {
            wallState = 0;
        }
        else if (transform.position == defaultPosition + openPosition)
        {
            wallState = 2;
            _boxCollider.center = new Vector3(0, 1, 0);
            _boxCollider.size = new Vector3(2, 2, 2);
        }
        else
        {
            wallState = 1;
        }

        // If this secret has been discovered in this save slot.
        if (_player.discoveredSecrets.Contains(_initialPositionToString) && wallState == 0)
        {
            InstantMove();
        }
    }

    #endregion

    #region Wall

    public IEnumerator MoveWall()
    {
        // Remember that this secret has been discovered.
        _player.discoveredSecrets.Add(_initialPositionToString);

        // Move the wall.
        Vector3 targetPos = defaultPosition + openPosition;

        // Expand collision to prevent player from getting in front of the wall's path.
        int sizeX = 2;
        int sizeZ = 2;
        float offsetX = 0;
        float offsetZ = 0;

        if (openPosition.x != 0)
        {
            sizeX = 4;
            offsetX = 1 * Mathf.Clamp(openPosition.x, -1f, 1f);
        }
        if (openPosition.z != 0)
        {
            sizeZ = 4;
            offsetZ = 1 * Mathf.Clamp(openPosition.z, -1f, 1f);
        }

        _boxCollider.size = new Vector3(sizeX, 2, sizeZ);
        _boxCollider.center = new Vector3(offsetX, 1, offsetZ);

        if (isSecret)
        {
            StaticClass.secretsDiscovered++;
            Player.scoreThisLevel += 3000;
            Player.gotScoreTimer = 4f;
        }

        if (soundOnMove != null)
        {
            var createdSound = Instantiate(soundOnMove, transform.position, transform.rotation);
            createdSound.transform.parent = transform.parent;
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.3f);

            yield return new WaitForSeconds(0.1f);
        }

        transform.position = targetPos;
    }

    public void InstantMove()
    {
        if (isSecret)
        {
            StaticClass.secretsDiscovered++;
        }

        transform.position = defaultPosition + openPosition;
        wallState = 2;
    }

    #endregion
}
