using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainGUIButton_KSH : MonoBehaviour, IPointerUpHandler
{
    public enum GUI_Type
    {
        None,
        Build,
        Dungeon,
        Hunter,
        Storage,
        Store
    }

    public GUI_Type gui_type;
    public UnityEngine.UI.Image bt_Img;
    public Sprite sprNoClick;
    public Sprite sprClicked;
    
    
    public UnityEngine.UI.Image bg_Img;
    public Sprite bgNoClick;
    public Sprite bgClicked;

    public TMP_Text bt_Text;
    public RectTransform rectTrans;
    public bool IsPressed = false;


    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressedCheck();
    }

    public void IsPressedCheck()
    {
        UnityEngine.Color color;

        if (IsPressed)
        {
            IsPressed = false;
            bt_Img.sprite = sprNoClick;
            bg_Img.sprite = bgNoClick;
            
            
            ColorUtility.TryParseHtmlString("#93D4B4", out color);
        }
        else
        {
            
            IsPressed = true;
            bt_Img.sprite = sprClicked;
            bg_Img.sprite = bgClicked;
            ColorUtility.TryParseHtmlString("#1E1710", out color);
        }
        rectTrans.sizeDelta = new Vector2(240, 240);
        bt_Text.color = color;
    }

    public void Action()
    {
        switch (gui_type)
        {
            case GUI_Type.Build: // 건설
                // 로직
                break;
            case GUI_Type.Dungeon: // 던전
                // 로직
                break;
            case GUI_Type.Hunter: // 헌터
                // 로직
                break;
            case GUI_Type.Storage: // 창고
                // 로직
                break;
            case GUI_Type.Store: // 상점
                // 로직
                break;
        }
    }
}
