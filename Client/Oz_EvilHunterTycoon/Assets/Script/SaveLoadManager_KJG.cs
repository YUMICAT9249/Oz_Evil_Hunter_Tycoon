using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveLoadManager_KJG : MonoBehaviour
{
    public static SaveLoadManager_KJG instance { get; private set; }

    private string savePath = Path.Combine(Application.persistentDataPath, "gameSave.json");

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //세이브 필요한 데이터들 여기에 추가
    [System.Serializable]
    public class SaveData
    {
        public int currentDifficultyLevel = 0;

        public double gold = 0;
        public long exp = 0;
        public int cash = 0;

        public float goldMultiplier = 1f;
        public float expMultiplier = 1f;
    }

    //게임 세이브 데이터 여기에 작성
    public void GameSave()
    {
        SaveData data = new SaveData();

        //게임 정보,배율 저장
        if (CurrencyManager_KJG.Instance != null)
        {
            data.gold = CurrencyManager_KJG.Instance.Gold;
            data.exp = CurrencyManager_KJG.Instance.Exp;
            data.cash = CurrencyManager_KJG.Instance.Cash;

            data.goldMultiplier = CurrencyManager_KJG.Instance.goldMultiplier;
            data.expMultiplier = CurrencyManager_KJG.Instance.expMultiplier;

        }

        //난이도 저장
        if (DifficultyManager_KJG.Instance != null)
        {
            data.currentDifficultyLevel = DifficultyManager_KJG.Instance.currentDifficultyLevel;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"Save Complite → {savePath}");
    }

    //게임 로드 데이터
    public void GameLoad()
    {
        if (!File.Exists(savePath))
        {
            NewGameSetup();
            return;
        }

        string json=File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (CurrencyManager_KJG.Instance != null)
        {
            CurrencyManager_KJG.Instance.Gold = data.gold;
            CurrencyManager_KJG.Instance.Exp = data.exp;
            CurrencyManager_KJG.Instance.Cash = data.cash;

            CurrencyManager_KJG.Instance.goldMultiplier = data.goldMultiplier;
            CurrencyManager_KJG.Instance.expMultiplier = data.expMultiplier;

            CurrencyManager_KJG.Instance.UpdateMultipliers();
        }

        if (DifficultyManager_KJG.Instance != null)
        {
            DifficultyManager_KJG.Instance.currentDifficultyLevel = data.currentDifficultyLevel;
        }
        Debug.Log($"Game Load Complite → Gold: {data.gold:N0}, DifficultyLevel: {data.currentDifficultyLevel}");
    }

    //게임 처음 시작할때 지급되는 자원
    private void NewGameSetup()
    {
        if (CurrencyManager_KJG.Instance != null)
        {
            CurrencyManager_KJG.Instance.Gold = 300;  // 시작 골드
            CurrencyManager_KJG.Instance.Exp = 0;
            CurrencyManager_KJG.Instance.Cash = 0;

            CurrencyManager_KJG.Instance.goldMultiplier = 1f;
            CurrencyManager_KJG.Instance.expMultiplier = 1f;
        }

        if (DifficultyManager_KJG.Instance != null)
        {
            DifficultyManager_KJG.Instance.currentDifficultyLevel = 0;
        }
    }
}
