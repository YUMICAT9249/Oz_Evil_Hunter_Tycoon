using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;
    public GameObject settingBG;

    

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
        }else if(settingBG.activeInHierarchy == true)
        {
            // 반대의 경우
            settingBG.SetActive(false);
        }
    }
}
