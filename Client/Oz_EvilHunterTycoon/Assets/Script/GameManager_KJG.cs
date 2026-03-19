using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_KJG : MonoBehaviour
{
   public static GameManager_KJG Instance {  get; private set; }

    public DataManager_KJG dataManager;
    public SaveLoadManager_KJG saveLoadManager;
    public CurrencyManager_KJG currencyManager;
    public DifficultyManager_KJG difficultyManager;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    private void InitializeManagers()
    {
        if (saveLoadManager != null) saveLoadManager.GameLoad();          // 1. 세이브 로드 먼저
        if (dataManager != null) dataManager.Initialize();                // 2. 데이터 테이블 초기화
        if (currencyManager != null) currencyManager.Initialize();        // 3. 화폐 초기화 (로드된 값 반영)
      //  if (difficultyManager != null) difficultyManager.Initialize();    // 4. 난이도 초기화
    }

    public void StartNewGame()
    {
        
    }
}
