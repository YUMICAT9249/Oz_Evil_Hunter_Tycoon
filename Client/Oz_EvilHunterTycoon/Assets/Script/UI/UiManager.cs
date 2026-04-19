using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    public enum MainGUISelectType
    {
        None,
        Build,
        Dungeon,
        HunterInfo,
        Storage,
        Store,
    }
    public MainGUISelectType MainGuiSelected; // 선택중인 GUI Type

    public GameObject settingBG;
    public GameObject HunterClickUI; // 헌터 누를 시 GUI
    public GameObject HunterInfoUI; // 헌터 정보 GUI
    public GameObject difficultyUI; // 난이도 선택 창
    public List<DifficultyButton_KSH> difficultyButtons = new List<DifficultyButton_KSH>();
    
    
    public GameObject MainGUIButton_Build;
    public GameObject MainGUIButton_Dungeon;
    public GameObject MainGUIButton_HunterInfo;
    public GameObject MainGUIButton_Storage;
    public GameObject MainGUIButton_Store;

    public GameObject DifficultyCheckWindow; // 난이도 설정 창

    public HunterData_PJS hunterData; // 헌터 데이터 베이스
    
    public int Difficulty = 1; // 현재 난이도
    public int OnDifficulty = 1; // 변경하려는 난이도

    public void Awake()
    {
        Instance = this;
        
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
        }
        else if (settingBG.activeInHierarchy == true)
        {
            // 반대의 경우
            settingBG.SetActive(false);
        }
    }

    public void OpenHunterInfoUI(HunterData_PJS hunterData = null)
    {
        if (HunterInfoUI.activeInHierarchy == false)
        {
            UI_HunterInfo_KSH hunterinfo = HunterInfoUI.GetComponent<UI_HunterInfo_KSH>();
            // 비활성 상태면 활성
            HunterInfoUI.SetActive(true);
            hunterinfo.ShowHunterInfo(hunterData);
        }
        else if (HunterInfoUI.activeInHierarchy == true)
        {
            // 반대의 경우
            HunterInfoUI.SetActive(false);
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
        else if (difficultyUI.activeInHierarchy == true)
        {
            // 반대의 경우
            Difficulty = OnDifficulty;
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
    

    public void DisableMainGUI()
    {
        MainGUIButton_KSH MGB_Build = MainGUIButton_Build.GetComponent<MainGUIButton_KSH>();
        MainGUIButton_KSH MGB_Dungeon = MainGUIButton_Dungeon.GetComponent<MainGUIButton_KSH>();
        MainGUIButton_KSH MGB_HunterInfo = MainGUIButton_HunterInfo.GetComponent<MainGUIButton_KSH>();
        MainGUIButton_KSH MGB_Storage = MainGUIButton_Storage.GetComponent<MainGUIButton_KSH>();
        MainGUIButton_KSH MGB_Store = MainGUIButton_Store.GetComponent<MainGUIButton_KSH>();

        MGB_Build.Disable();
        MGB_Dungeon.Disable();
        MGB_HunterInfo.Disable();
        MGB_Storage.Disable();
        MGB_Store.Disable();
    }
    public void TargetHunter(bool isActive, HunterController_PJS hunter = null)
    {
        // 카메라 타겟 지정

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
