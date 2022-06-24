using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    Vector3 defaultPosition;
    public Vector3 openPosition;
    public int wallState;
    public GameObject soundOnMove;

    void Start()
    {
        defaultPosition = transform.position;
    }

    void Update()
    {
        if (transform.position == defaultPosition)
        {
            wallState = 0;
        }
        else if (transform.position == openPosition)
        {
            wallState = 2;
        }
        else
        {
            wallState = 1;
        }
    }

    public IEnumerator MoveWall()
    {
        Vector3 targetPos = defaultPosition + openPosition;

        if (soundOnMove != null)
        {
            Instantiate(soundOnMove, transform.position, transform.rotation);
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.2f);

            yield return new WaitForSeconds(0.1f);
        }

        transform.position = targetPos;
    }
}
