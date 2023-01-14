using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject fade;
    public GameObject useSound;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("ExitDefault");
    }

    public void UsedExit()
    {
        anim.Play("ExitUsed");

        if(useSound != null)
        {
            Instantiate(useSound, transform.position, transform.rotation);
        }
    }
}
