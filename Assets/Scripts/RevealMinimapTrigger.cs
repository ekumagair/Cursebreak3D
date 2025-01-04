using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealMinimapTrigger : MonoBehaviour
{
    private Minimap _minimapScript;

    void Start()
    {
        _minimapScript = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HUD>().mapRoot.GetComponent<Minimap>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null)
        {
            _minimapScript.AddToMinimapFilter(other.gameObject);
        }
    }
}
