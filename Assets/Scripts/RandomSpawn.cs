using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject[] objectToSpawn;

    void Start()
    {
        Instantiate(objectToSpawn[Random.Range(0, objectToSpawn.Length)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
