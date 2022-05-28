using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TargetWindowController : MonoBehaviour
{
    private Animator animator;

    private EventSystem system;

    private Button ManSelect;
    private Button WolfSelect;
    private Button ZombieSelect;
    private Button CloseButton;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        system = GetComponent<EventSystem>();

        ManSelect = transform.Find("ManSelect").gameObject.GetComponent<Button>();
        WolfSelect = transform.Find("WolfSelect").gameObject.GetComponent<Button>();
        ZombieSelect = transform.Find("ZombieSelect").gameObject.GetComponent<Button>();
        CloseButton = transform
            .Find("CloseButton")
            .gameObject.transform.Find("Button")
            .gameObject.GetComponent<Button>();

        ManSelect.onClick.AddListener(ManSelected);
        WolfSelect.onClick.AddListener(WolfSelected);
        ZombieSelect.onClick.AddListener(ZombieSelected);
        CloseButton.onClick.AddListener(CloseButtonDown);
    }

    private void ManSelected()
    {
        //设置道具卡使用目标
        MonopolyGame.CardTarget = 0;
        //一旦道具卡使用，销毁其物体
        DestoryCard();
        UIEvent.TargetWindowClose = true; //关闭目标选择窗口
    }

    private void WolfSelected()
    {
        MonopolyGame.CardTarget = 1;
        DestoryCard();
        UIEvent.TargetWindowClose = true;
    }

    private void ZombieSelected()
    {
        MonopolyGame.CardTarget = 2;
        DestoryCard();
        UIEvent.TargetWindowClose = true;
    }

    private void DestoryCard()
    {
        MonopolyGame.Card.RemoveAt(CardController.idex);
        MonopolyGame.CardType.RemoveAt(CardController.idex);
        Destroy(CardController.CardUsed);
    }

    private void Update()
    {
        SwitchAnimation();
        //SwitchNavigation();
        //通过在animation中设置脚本是否可用来结局
    }

    private void SwitchAnimation()
    {
        animator.SetBool("Show", UIEvent.TargetWindowShow);
        if (UIEvent.TargetWindowShow)
            UIEvent.TargetWindowShow = false;
        animator.SetBool("Close", UIEvent.TargetWindowClose);
        if (UIEvent.TargetWindowClose)
            UIEvent.TargetWindowClose = false;
    }

    private void SwitchNavigation()
    {
        GameObject obj = system.currentSelectedGameObject;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (obj == null)
                ManSelect.Select();
        }
    }

    private void CloseButtonDown()
    {
        MonopolyGame.CardUsed = -1;
        UIEvent.TargetWindowClose = true;
    }
}
