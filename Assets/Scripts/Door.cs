using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Vector3 closedPosition;
    public Vector3 openPosition;
    public int doorState;
    public GameObject soundObject;

    void Start()
    {
        closedPosition = transform.position;
    }

    void Update()
    {
        if(transform.position == closedPosition)
        {
            doorState = 0;
        }
        else if (transform.position == openPosition)
        {
            doorState = 2;
        }
        else
        {
            doorState = 1;
        }
    }

    public IEnumerator OpenDoor()
    {
        StartCoroutine(MoveDoor(closedPosition + openPosition));

        yield return new WaitForSeconds(8f);

        StartCoroutine(MoveDoor(closedPosition));
    }

    IEnumerator MoveDoor(Vector3 target)
    {
        Instantiate(soundObject, transform.position, transform.rotation);

        while (Vector3.Distance(transform.position, target) > 0.001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 200f * Time.deltaTime);

            yield return new WaitForSeconds(0.05f);
        }

        transform.position = target;
    }
}
