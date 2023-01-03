using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealMinimapTrigger : MonoBehaviour
{
    Minimap minimapScript;

    void Start()
    {
        minimapScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>().mapRoot.GetComponent<Minimap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != null)
        {
            minimapScript.AddToMinimapFilter(other.gameObject);
        }
    }
}
