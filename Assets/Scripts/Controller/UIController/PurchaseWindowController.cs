using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseWindowController : MonoBehaviour
{
    // Start is called before the first frame update
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
        animator.SetBool("Show", UIEvent.PurchaseWindowShow);
        if (UIEvent.PurchaseWindowShow) UIEvent.PurchaseWindowShow = false;
        animator.SetBool("Close", UIEvent.PurchaseWindowClose);
        if (UIEvent.PurchaseWindowClose) UIEvent.PurchaseWindowClose = false;
    }
}
