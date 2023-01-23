using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    Vector3 defaultPosition;
    public Vector3 openPosition;
    public int wallState;
    public GameObject soundOnMove;
    [Tooltip("Whether this wall counts as a secret or not. Moving walls that are necessary for progression shouldn't count as secrets.")]
    public bool isSecret = true;
    public bool canBeAddedToMinimap = true;

    string initialPositionToString;
    Player player;
    BoxCollider boxCollider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        boxCollider = GetComponent<BoxCollider>();

        defaultPosition = transform.position;
        initialPositionToString = transform.position.x.ToString() + transform.position.y.ToString() + transform.position.z.ToString();

        if (isSecret)
        {
            StaticClass.secretsTotal++;
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
            boxCollider.center = new Vector3(0, 1, 0);
            boxCollider.size = new Vector3(2, 2, 2);
        }
        else
        {
            wallState = 1;
        }

        // If this secret has been discovered in this save slot.
        if (player.discoveredSecrets.Contains(initialPositionToString) && wallState == 0)
        {
            InstantMove();
        }
    }

    public IEnumerator MoveWall()
    {
        // Remember that this secret has been discovered.
        player.discoveredSecrets.Add(initialPositionToString);

        // Move the wall.
        Vector3 targetPos = defaultPosition + openPosition;

        // Expand collision to prevent player from getting in front of the wall's path.
        int sizeX = 2;
        int sizeZ = 2;
        float offsetX = 0;
        float offsetZ = 0;

        if(openPosition.x != 0)
        {
            sizeX = 4;
            offsetX = 1 * Mathf.Clamp(openPosition.x, -1f, 1f);
        }
        if (openPosition.z != 0)
        {
            sizeZ = 4;
            offsetZ = 1 * Mathf.Clamp(openPosition.z, -1f, 1f);
        }

        boxCollider.size = new Vector3(sizeX, 2, sizeZ);
        boxCollider.center = new Vector3(offsetX, 1, offsetZ);

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
}
