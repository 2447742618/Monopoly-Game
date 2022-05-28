using UnityEngine;
using UnityEngine.UI;

public class UIEvent : MonoBehaviour
{
    public static bool InitWindowClose = false;
    public static bool InitWindowShow = false;
    public static bool InitFinish = false;

    public static bool PurchaseWindowShow = false;
    public static bool PurchaseWindowClose = false;
    public static bool PurchaseFinish = false;

    public static bool TargetWindowShow = false;
    public static bool TargetWindowClose = false;

    public static bool SystemMessageShow = false;

    public static string PlayerName = "";
    public static int CharacterSelect = 0; //角色选择

    public void GoButtonDown()
    {
        for(int i=0;i<MonopolyGame.Card.Count;i++)
        {
            GameObject obj=(GameObject)MonopolyGame.Card[i];
            obj.GetComponent<Button>().interactable=false;//按下GO之后道具卡不再可用
        }
        MonopolyGame.GoButton.GetComponent<Button>().interactable=false;
        MonopolyGame.RollReady = true;
    }

    public static void ExitButtonDown()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void InitWindowCloseButtonDown()
    {
        InitWindowClose = true;
        InitFinish = true;
    }

    public void PlayerNameEnter()
    {
        PlayerName = GameObject.FindWithTag("PlayerName").GetComponent<Text>().text;
    }

    public void PurchaseConfirmButtonDown()
    {
        MonopolyGame.PurchaseConfirm = true;
        PurchaseWindowClose = true;
        PurchaseFinish = true;
    }

    public void PurchaseWindowCLoseButtonDown()
    {
        PurchaseWindowClose = true;
        PurchaseFinish = true;
    }
}
