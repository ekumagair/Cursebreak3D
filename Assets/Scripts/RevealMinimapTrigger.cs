using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealMinimapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null)
        {
            HUD.Instance.Minimap.AddToMinimapFilter(other.gameObject);
        }
    }
}
