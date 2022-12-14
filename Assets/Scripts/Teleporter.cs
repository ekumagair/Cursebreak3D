using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleporter : MonoBehaviour
{
    public int teleporterAnimation = 1;
    public GameObject soundOnTeleport;
    public GameObject particleOnTeleport;
    public GameObject teleporterDestination;
    public bool changeRotation;
    public bool playerCanUse = true;
    public bool enemyCanUse = true;

    void Start()
    {
        Debug.Log(gameObject + " destination: " + teleporterDestination.transform.position);
        GetComponent<Animator>().Play("Teleporter" + teleporterAnimation.ToString());
    }

    public void Teleport(GameObject user)
    {
        SetControlScripts(user, false);

        user.transform.position = new Vector3(teleporterDestination.transform.position.x, user.transform.position.y, teleporterDestination.transform.position.z);

        if(changeRotation)
        {
            user.transform.rotation = teleporterDestination.transform.rotation;
        }

        if (soundOnTeleport != null)
        {
            Instantiate(soundOnTeleport, transform.position, transform.rotation);
            Instantiate(soundOnTeleport, teleporterDestination.transform.position, teleporterDestination.transform.rotation);
        }
        if(particleOnTeleport != null)
        {
            Instantiate(particleOnTeleport, transform.position + (transform.up * 2), transform.rotation);
            Instantiate(particleOnTeleport, teleporterDestination.transform.position + (user.transform.forward * 1.5f) + (transform.up * 2), teleporterDestination.transform.rotation);
        }

        SetControlScripts(user, true);

        Debug.Log(gameObject + " teleported " + user);
    }

    // Some additional control scripts need to be disabled temporarily to make the teleportation work.
    void SetControlScripts(GameObject obj, bool changeTo)
    {
        if (obj.GetComponent<CharacterController>() != null)
        {
            obj.GetComponent<CharacterController>().enabled = changeTo;
        }
        if (obj.GetComponent<NavMeshAgent>() != null)
        {
            obj.GetComponent<NavMeshAgent>().enabled = changeTo;
        }
    }
}
