using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemMessageController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        SwitchAnimation();   
    }

    private void SwitchAnimation()
    {
        animator.SetBool("Show", UIEvent.SystemMessageShow);
        if (UIEvent.SystemMessageShow) UIEvent.SystemMessageShow = false;
    }
}
