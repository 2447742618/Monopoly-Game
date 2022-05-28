using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    private Button NewGameButton;
    private Button ContinueButton;
    private Button ExitButton;

    private Animator animator;

    EventSystem system;

    private bool NewGame = false;

    private void Awake()
    {
        system = EventSystem.current;

        animator = GetComponent<Animator>();

        NewGameButton = transform.GetChild(1).GetComponent<Button>();
        ContinueButton = transform.GetChild(2).GetComponent<Button>();
        ExitButton = transform.GetChild(3).GetComponent<Button>();

        NewGameButton.onClick.AddListener(NewGameButtonDown);
        ExitButton.onClick.AddListener(UIEvent.ExitButtonDown);
    }

    private void NewGameButtonDown()
    {
        MonopolyGame.GameBegin = true;
        system.SetSelectedGameObject(null);
        NewGame = true;
    }

    private void Update()
    {
        SwitchAnimation();
        SwitchNavigation();
    }

    private void SwitchAnimation()
    {
        animator.SetBool("Action", NewGame);
        if (NewGame) NewGame = false;
    }

    private void SwitchNavigation()
    {
        GameObject obj = system.currentSelectedGameObject;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (obj == null)
            {
                NewGameButton.Select();
            }
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (obj == NewGameButton.GetComponent<Selectable>().gameObject) ExitButton.Select();
            else if (obj == ContinueButton.GetComponent<Selectable>().gameObject) NewGameButton.Select();
            else if (obj == ExitButton.GetComponent<Selectable>().gameObject) ContinueButton.Select();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (obj == ContinueButton.GetComponent<Selectable>().gameObject) ExitButton.Select();
            else if (obj == ExitButton.GetComponent<Selectable>().gameObject) NewGameButton.Select();
            else if (obj == NewGameButton.GetComponent<Selectable>().gameObject) ContinueButton.Select();
        }
    }
}
