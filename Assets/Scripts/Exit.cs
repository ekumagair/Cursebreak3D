using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject fade;

    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.Play("ExitDefault");
    }

    public void UsedExit()
    {
        anim.Play("ExitUsed");
    }
}
