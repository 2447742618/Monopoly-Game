using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public static GameObject CardUsed = null;
    public static int idex=-1;

    public static void UseCard()
    {
        GameObject obj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.gameObject;
        idex = obj.transform.GetSiblingIndex();
        MonopolyGame.CardUsed = (int)MonopolyGame.CardType[idex];
        UIEvent.TargetWindowShow = true;
        CardUsed = obj;//用于使用道具卡后删除对应游戏实体
    }
}
