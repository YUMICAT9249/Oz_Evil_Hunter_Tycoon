using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    public GameObject settingBG;
    public GameObject HunterClickUI; // 헌터 누를 시 GUI
    public GameObject difficultyUI; // 난이도 선택 창
    public List<DifficultyButton_KSH> difficultyButtons = new List<DifficultyButton_KSH>();
    public DifficultyButton_KSH d1;
    public DifficultyButton_KSH d2;
    public DifficultyButton_KSH d3;

    public int Difficulty = 1;

    public void Awake()
    {
        Instance = this;
        difficultyButtons.Add(d1);
        difficultyButtons.Add(d2);
        difficultyButtons.Add(d3);
    }

    public void SettingBG()
    {
        if (settingBG == null) 
        {
            Debug.Log("SettingBG 미할당");
            return; 
        }
        if (settingBG.activeInHierarchy == false)
        {
            // 설정창이 비활성 상태면 활성
            settingBG.SetActive(true);
        }else if(settingBG.activeInHierarchy == true)
        {
            // 반대의 경우
            settingBG.SetActive(false);
        }
    }

    public void UI_Difficulty()
    {
        if (difficultyUI == null)
        {
            Debug.Log("난이도 설정 창 미할당");
            return;
        }
        if (difficultyUI.activeInHierarchy == false)
        {
            // 난이도 설정창이 비활성 상태면 활성
            difficultyUI.SetActive(true);
            RefreshDifficultyButton();


        }
        else if (settingBG.activeInHierarchy == true)
        {
            // 반대의 경우
            difficultyUI.SetActive(false);
        }
    }

    public void RefreshDifficultyButton()
    {
        foreach (DifficultyButton_KSH button in difficultyButtons)
        {
            button.SetActive();
        }
    }
    public void TargetHunter(bool isActive, HunterUIAction_KSH hunter = null)
    {
        if (isActive)
        {
            UserCameraMove_YHJ.Instance.TargetHunter(hunter);
            HunterClickUI.SetActive(true);
        }
        else
        {
            UserCameraMove_YHJ.Instance.NoTarget();
            HunterClickUI.SetActive(false);
        }
    }
}
