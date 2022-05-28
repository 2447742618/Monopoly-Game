using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InitWindowController : MonoBehaviour
{
    private Animator animator;

    private EventSystem system;

    private Button ManSelect = null;
    private Button WolfSelect = null;
    private Button ZombieSelect = null;
    private Button Finish = null;
    private InputField input = null;

    private void Awake()
    {
        system = EventSystem.current;

        animator = GetComponent<Animator>();

        ManSelect = transform.Find("ManSelect").gameObject.GetComponent<Button>();
        WolfSelect = transform.Find("WolfSelect").gameObject.GetComponent<Button>();
        ZombieSelect = transform.Find("ZombieSelect").gameObject.GetComponent<Button>();
        Finish = transform.Find("FinishButton").gameObject.GetComponent<Button>();
        input = transform.Find("InputField").gameObject.GetComponent<InputField>();

        ManSelect.onClick.AddListener(ManSelected);
        WolfSelect.onClick.AddListener(WolfSelected);
        ZombieSelect.onClick.AddListener(ZombieSelected);
    }

    private void Update()
    {
        SwitchAnimation();
        SwitchNavigation();
    }

    private void PhotoSelected(Button button)
    {
        Image SelectedMark = button.transform.Find("SelectedMark").gameObject.GetComponent<Image>();
        SelectedMark.color = new Color(255, 255, 255, 255);
        ManSelect.interactable = false;
        WolfSelect.interactable = false;
        ZombieSelect.interactable = false;
    }

    private void ManSelected()
    {
        UIEvent.CharacterSelect = 0;
        PhotoSelected(ManSelect);
    }

    private void WolfSelected()
    {
        UIEvent.CharacterSelect = 1;
        PhotoSelected(WolfSelect);
    }

    private void ZombieSelected()
    {
        UIEvent.CharacterSelect = 2;
        PhotoSelected(ZombieSelect);
    }

    private void SwitchAnimation()
    {
        animator.SetBool("Close", UIEvent.InitWindowClose);
        if (UIEvent.InitWindowClose) UIEvent.InitWindowClose = false;
        animator.SetBool("Show", UIEvent.InitWindowShow);
        if (UIEvent.InitWindowShow) UIEvent.InitWindowShow = false;
    }

    private void SwitchNavigation()
    {
        GameObject obj = system.currentSelectedGameObject;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (obj == null)
            {
                if (ManSelect.interactable) ManSelect.Select();
                else input.Select();
            }
            else if (obj == ManSelect.GetComponent<Selectable>().gameObject || obj == WolfSelect.GetComponent<Selectable>().gameObject || obj == ZombieSelect.GetComponent<Selectable>().gameObject) input.Select();
            else if (input.isFocused) Finish.Select();
            else if (obj == Finish.GetComponent<Selectable>().gameObject)
            {
                if (ManSelect.interactable) ManSelect.Select();
                else input.Select();
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (obj == ManSelect.GetComponent<Selectable>().gameObject || obj == WolfSelect.GetComponent<Selectable>().gameObject || obj == ZombieSelect.GetComponent<Selectable>().gameObject) input.Select();
            else if (input.isFocused) Finish.Select();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (obj == Finish.GetComponent<Selectable>().gameObject) input.Select();
            else if (input.isFocused && ManSelect.interactable) ManSelect.Select();
        }
    }

}
