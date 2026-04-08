using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyButton_KSH : MonoBehaviour, IPointerUpHandler
{
    public GameObject SelectImage;

    public enum ButtonDifficulty_Type
    {
        None = 0,
        Easy,
        Normal,
        Hard,
    }
    public ButtonDifficulty_Type buttonDifficulty; // 버튼 난이도


    public void SetActive()
    {
        if(UiManager.Instance.Difficulty == (int)buttonDifficulty)
        {
            SelectImage.SetActive(true);
        }
        else
        {
            SelectImage.SetActive(false);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UiManager.Instance.Difficulty = (int)buttonDifficulty;
        UiManager.Instance.RefreshDifficultyButton();
    }
}
