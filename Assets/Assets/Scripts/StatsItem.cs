using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatsItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string Message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.ShowStatInfo(Message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.CloseStatInfo();
    }
}
