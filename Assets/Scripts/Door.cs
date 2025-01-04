using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    #region Variables

    private Vector3 closedPosition;
    public Vector3 openPosition;
    public int doorState;
    public GameObject soundObject;
    public GameObject soundObjectLocked;
    public int key = 0;
    public bool canUse = true;

    #endregion

    // Door states
    // 0 = Closed
    // 1 = Moving
    // 2 = Open

    #region Default Methods

    void Start()
    {
        closedPosition = transform.position;
        canUse = true;
    }

    void Update()
    {
        if(transform.position == closedPosition)
        {
            StopCoroutine(MoveDoor(closedPosition));
            doorState = 0;
        }
        else if (transform.position == openPosition)
        {
            StopCoroutine(MoveDoor(closedPosition + openPosition));
            doorState = 2;
        }
        else
        {
            doorState = 1;
        }
    }

    #endregion

    #region Door

    public IEnumerator OpenDoor()
    {
        if (doorState == 0 && canUse == true)
        {
            GameObject user = GameObject.FindGameObjectWithTag("Player");

            if (user.GetComponent<Player>().keys[key] == true)
            {
                StartCoroutine(MoveDoor(closedPosition + openPosition));

                yield return new WaitForSeconds(8f);

                StartCoroutine(MoveDoor(closedPosition));
            }
            else
            {
                Debug.Log("Door locked!");

                if (soundObjectLocked != null)
                {
                    Instantiate(soundObjectLocked, transform.position, transform.rotation);
                }
            }
        }
    }

    IEnumerator MoveDoor(Vector3 target)
    {
        canUse = false;

        if (soundObject != null)
        {
            Instantiate(soundObject, transform.position, transform.rotation);
        }

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 0.75f);

            yield return new WaitForSeconds(0.1f);
        }

        transform.position = target;

        yield return new WaitForSeconds(0.4f);

        canUse = true;
    }

    #endregion
}
