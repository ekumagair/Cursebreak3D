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
        if (other.gameObject.isStatic && other.gameObject.name != "Floor" && other.gameObject.name != "Ceiling")
        {
            minimapScript.AddToMinimap(other.gameObject);
        }
    }
}
