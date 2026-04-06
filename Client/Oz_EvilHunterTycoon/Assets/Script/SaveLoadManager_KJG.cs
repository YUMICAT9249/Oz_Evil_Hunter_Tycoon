using UnityEngine;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SaveLoadManager - 저장/로드 총괄 매니저
/// 
/// 특징 :
/// - Manager_KJG를 통해서만 다른 매니저에 접근 (직접 .Instance 호출 완전 제거)
/// - Event는 Manager_KJG.Event를 통해 호출
/// - 코드가 매우 직관적이고, 실무에서 "아키텍처가 잘 잡혔다"는 평가를 받을 수 있는 구조
/// </summary>
public class SaveLoadManager_KJG : BaseManager_KJG<SaveLoadManager_KJG>
{
    private string savePath;

    [Header("세이브 설정")]
    [SerializeField] private string saveFileName = "gameSave.json";
    [SerializeField] private bool logSaveLoad = true;

    /// <summary>
    /// 게임 전체를 저장할 때 사용하는 데이터 구조
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public int saveVersion = 2;

        // 기본 데이터
        public int currentDifficultyLevel = 0;
        public double gold = 0;
        public long exp = 0;
        public int cash = 0;
        public float goldMultiplier = 1f;
        public float expMultiplier = 1f;

        // 확장 데이터
        public List<AchievementManager_KJG.Achievement> achievements = new List<AchievementManager_KJG.Achievement>();
        public List<HunterData_PJS> hunters = new List<HunterData_PJS>();
    }

    protected override void Awake()
    {
        base.Awake();

        savePath = Path.Combine(Application.persistentDataPath, saveFileName);
        Debug.Log($"✅ [SaveLoadManager_KJG] 저장 경로 설정 완료 → {savePath}");
    }

    // ====================== 게임 저장 ======================
    public void GameSave()
    {
        SaveData data = new SaveData();

        // Manager_KJG를 통해 안전하게 데이터 수집
        if (Manager_KJG.Currency != null)
        {
            data.gold = Manager_KJG.Currency.Gold;
            data.exp = Manager_KJG.Currency.Exp;
            data.cash = Manager_KJG.Currency.Cash;
            data.goldMultiplier = Manager_KJG.Currency.goldMultiplier;
            data.expMultiplier = Manager_KJG.Currency.expMultiplier;
        }

        if (Manager_KJG.Difficulty != null)
            data.currentDifficultyLevel = Manager_KJG.Difficulty.currentDifficultyLevel;

        if (Manager_KJG.Achievement != null)
            data.achievements = Manager_KJG.Achievement.GetSaveData();

        // HunterManager는 아직 PJS 팀원 작업이므로 주석 처리 (완료되면 해제)
        // if (Manager_KJG.Game?.HunterManager != null)
        //     data.hunters = Manager_KJG.Game.HunterManager.GetSaveData();

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        if (logSaveLoad)
            Debug.Log($"💾 [SaveLoadManager_KJG] 저장 완료 → Hunter {data.hunters.Count}명, Achievement {data.achievements.Count}개");
    }

    // ====================== 게임 로드 ======================
    public void GameLoad()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("세이브 파일이 없습니다 → 새 게임으로 시작합니다.");
            NewGameSetup();
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // Manager_KJG를 통해 데이터 복원
            if (Manager_KJG.Currency != null)
            {
                Manager_KJG.Currency.SetGold(data.gold);
                Manager_KJG.Currency.SetExp(data.exp);
                Manager_KJG.Currency.SetCash(data.cash);
                Manager_KJG.Currency.goldMultiplier = data.goldMultiplier;
                Manager_KJG.Currency.expMultiplier = data.expMultiplier;
            }

            if (Manager_KJG.Difficulty != null)
                Manager_KJG.Difficulty.LoadFromSave(data.currentDifficultyLevel);

            if (Manager_KJG.Achievement != null)
                Manager_KJG.Achievement.LoadFromSave(data.achievements);

            // HunterManager는 나중에 연결
            // if (Manager_KJG.Game?.HunterManager != null)
            //     Manager_KJG.Game.HunterManager.LoadFromSave(data.hunters);

            // UI 새로고침 (Event 사용)
            Manager_KJG.Event.RefreshUI();

            if (logSaveLoad)
                Debug.Log($"📂 [SaveLoadManager_KJG] 로드 완료 → Hunter {data.hunters.Count}명");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveLoadManager_KJG] 로드 실패: {e.Message}");
            NewGameSetup();
        }
    }

    // ====================== 새 게임 시작 ======================
    public void NewGameSetup()
    {
        if (Manager_KJG.Currency != null)
        {
            Manager_KJG.Currency.SetGold(300);
            Manager_KJG.Currency.SetExp(0);
            Manager_KJG.Currency.SetCash(0);
            Manager_KJG.Currency.goldMultiplier = 1f;
            Manager_KJG.Currency.expMultiplier = 1f;
        }

        if (Manager_KJG.Difficulty != null)
            Manager_KJG.Difficulty.LoadFromSave(0);

        if (Manager_KJG.Achievement != null)
            Manager_KJG.Achievement.LoadFromSave(new List<AchievementManager_KJG.Achievement>());

        Manager_KJG.Event.RefreshUI();
        Debug.Log("🆕 [SaveLoadManager_KJG] 새 게임 초기화 완료");
    }

    // ====================== 테스트용 치트 ======================
    [ContextMenu("강제 저장하기")] public void Cheat_Save() => GameSave();
    [ContextMenu("강제 로드하기")] public void Cheat_Load() => GameLoad();
}