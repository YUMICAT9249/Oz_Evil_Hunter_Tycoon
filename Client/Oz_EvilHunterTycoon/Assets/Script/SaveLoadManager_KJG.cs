using UnityEngine;
using System.IO;

public class SaveLoadManager_KJG : MonoBehaviour
{
    public static SaveLoadManager_KJG Instance { get; private set; }

    private string savePath;

    [Header("세이브 설정")]
    [SerializeField] private string saveFileName = "gameSave.json";
    [SerializeField] private bool logSaveLoad = true;

    [System.Serializable]
    public class SaveData
    {
        public int saveVersion = 1;
        public int currentDifficultyLevel = 0;
        public double gold = 0;
        public long exp = 0;
        public int cash = 0;
        public float goldMultiplier = 1f;
        public float expMultiplier = 1f;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        Debug.Log($"✅ SaveLoadManager_KJG 초기화 완료 | 저장 경로: {savePath}");
    }

    // ====================== 세이브 ======================
    public void GameSave()
    {
        SaveData data = new SaveData();

        // CurrencyManager에서 데이터 가져오기
        if (CurrencyManager_KJG.Instance != null)
        {
            data.gold = CurrencyManager_KJG.Instance.Gold;
            data.exp = CurrencyManager_KJG.Instance.Exp;
            data.cash = CurrencyManager_KJG.Instance.Cash;
            data.goldMultiplier = CurrencyManager_KJG.Instance.goldMultiplier;
            data.expMultiplier = CurrencyManager_KJG.Instance.expMultiplier;
        }

        // DifficultyManager에서 데이터 가져오기
        if (DifficultyManager_KJG.Instance != null)
        {
            data.currentDifficultyLevel = DifficultyManager_KJG.Instance.currentDifficultyLevel;
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        if (logSaveLoad)
            Debug.Log($"💾 게임 저장 완료 → {savePath}");

        // 저장 완료 후 글로벌 이벤트 발생
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RequestSave);
    }

    // ====================== 로드 ======================
    public void GameLoad()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("세이브 파일이 없습니다. 새 게임을 시작합니다.");
            NewGameSetup();
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // CurrencyManager에 데이터 적용
            if (CurrencyManager_KJG.Instance != null)
            {
                CurrencyManager_KJG.Instance.SetGold(data.gold);
                CurrencyManager_KJG.Instance.SetExp(data.exp);
                CurrencyManager_KJG.Instance.SetCash(data.cash);
                CurrencyManager_KJG.Instance.goldMultiplier = data.goldMultiplier;
                CurrencyManager_KJG.Instance.expMultiplier = data.expMultiplier;
            }

            // DifficultyManager에 데이터 적용
            if (DifficultyManager_KJG.Instance != null)
            {
                DifficultyManager_KJG.Instance.LoadFromSave(data.currentDifficultyLevel);
            }

            // 로드 완료 후 이벤트 발생
            EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RefreshUI);

            if (logSaveLoad)
                Debug.Log($"📂 게임 로드 완료 → Gold: {data.gold:N0} | Cash: {data.cash} | Difficulty: {data.currentDifficultyLevel}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"세이브 파일 로드 중 오류 발생: {e.Message}");
            NewGameSetup();
        }
    }

    // ====================== 새 게임 초기화 ======================
    public void NewGameSetup()
    {
        if (CurrencyManager_KJG.Instance != null)
        {
            CurrencyManager_KJG.Instance.SetGold(300);
            CurrencyManager_KJG.Instance.SetExp(0);
            CurrencyManager_KJG.Instance.SetCash(0);
            CurrencyManager_KJG.Instance.goldMultiplier = 1f;
            CurrencyManager_KJG.Instance.expMultiplier = 1f;
        }

        if (DifficultyManager_KJG.Instance != null)
        {
            DifficultyManager_KJG.Instance.LoadFromSave(0);
        }

        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RefreshUI);

        Debug.Log("🆕 새 게임 초기화 완료");
    }

    // ====================== 편의 기능 (치트) ======================
    [ContextMenu("강제 저장하기")]
    public void Cheat_Save() => GameSave();

    [ContextMenu("강제 로드하기")]
    public void Cheat_Load() => GameLoad();

    [ContextMenu("세이브 파일 삭제하기")]
    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("🗑 세이브 파일이 삭제되었습니다.");
        }
    }
}