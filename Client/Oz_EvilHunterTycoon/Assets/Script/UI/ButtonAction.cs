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
        Setting,
        CameraNoTarget,
        DifficultySet,
        DifficultySetCheck,
        DifficultyClose,
        HunterInfoUI,
    }

    public buttionActionType actionType;

    public void OnPointerUp(PointerEventData eventData)
    {
        switch ((buttionActionType)actionType)
        {
            case buttionActionType.TouchToStart:
                LoadingManager.LoadScene("Ingame_Scene");
                break;
            case buttionActionType.Setting:
                UiManager.Instance.SettingBG();
                break;
            case buttionActionType.CameraNoTarget:
                UiManager.Instance.TargetHunter(false);
                break;
            case buttionActionType.DifficultySet:
                UiManager.Instance.UI_Difficulty();
                break;
            case buttionActionType.DifficultySetCheck:
                UiManager.Instance.DifficultyCheckWindow.SetActive(true);
                break;
            case buttionActionType.HunterInfoUI:
                UiManager.Instance.OpenHunterInfoUI();
                break;



        }
    }
}
