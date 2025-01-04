using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    #region Variables

    public GameObject fade;
    public GameObject useSound;

    private Animator _anim;

    #endregion

    #region Default Methods

    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.Play("ExitDefault");
    }

    public void UsedExit()
    {
        _anim.Play("ExitUsed");

        if (useSound != null)
        {
            Instantiate(useSound, transform.position, transform.rotation);
        }
    }

    #endregion
}
