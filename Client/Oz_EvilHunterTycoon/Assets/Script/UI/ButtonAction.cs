using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAction : MonoBehaviour, IPointerUpHandler
{
    public enum buttionActionType
    {
        None = 0,
        TouchToStart,
    }

    public buttionActionType actionType;

    public void OnPointerUp(PointerEventData eventData)
    {
        switch ((buttionActionType)actionType)
        {
            case buttionActionType.TouchToStart:
                LoadingManager.LoadScene("Ingame_Scene");
                break;
        }
    }
}
