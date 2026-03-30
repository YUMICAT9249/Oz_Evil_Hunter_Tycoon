using UnityEngine;
using System;

public class DifficultyManager_KJG : MonoBehaviour
{
    public static DifficultyManager_KJG Instance { get; private set; }

    [Header("현재 난이도")]
    public int currentDifficultyLevel = 0;

    private readonly string[] difficultyNames =
    {
        "Easy", "Normal", "Hard"
    };

    // 다음 난이도 업그레이드에 필요한 환생 헌터 수
    private readonly int[] requiredReincarnatedHunters =
    {
        0,  // Easy
        5,  // Normal
        10  // Hard
    };

    // 난이도별 배율 (골드, 경험치)
    public float[] goldMultiplierPerDifficulty = { 1f, 1.5f, 2.2f };
    public float[] expMultiplierPerDifficulty = { 1f, 1.5f, 1.2f };

    // 몬스터 스탯 배율
    public float monsterHpMultiplier => 1f + (currentDifficultyLevel * 0.8f);
    public float monsterAtkMultiplier => 1f + (currentDifficultyLevel * 0.7f);

    // ==================== C# Event (당신이 원하는 방식) ====================
    public event Action<int> OnDifficultyChanged;        // 난이도 변경 시 (현재 레벨 전달)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ DifficultyManager_KJG 초기화 완료");
    }

    // ==================== 난이도 관련 메서드 ====================

    /// <summary> 현재 난이도 이름 반환 </summary>
    public string GetCurrentDifficultyName()
    {
        if (currentDifficultyLevel < 0 || currentDifficultyLevel >= difficultyNames.Length)
            return "Unknown";

        return difficultyNames[currentDifficultyLevel];
    }

    /// <summary> 현재 난이도 업그레이드 가능 여부 </summary>
    public bool CanUpgradeDifficulty()
    {
        if (currentDifficultyLevel >= difficultyNames.Length - 1)
            return false;

        // HunterManager와 연결 후 사용 (현재는 주석 처리)
        // int required = requiredReincarnatedHunters[currentDifficultyLevel];
        // int currentReincarnated = HunterManager_KJG.Instance?.GetReincarnatedHunterCount() ?? 0;
        // return currentReincarnated >= required;

        return true; // 임시로 항상 true (테스트용)
    }

    /// <summary> 난이도 업그레이드 실행 </summary>
    public void UpgradeDifficulty()
    {
        if (!CanUpgradeDifficulty())
        {
            Debug.LogWarning("난이도 업그레이드가 불가능합니다.");
            return;
        }

        currentDifficultyLevel++;

        Debug.Log($"난이도 업그레이드 완료! → {GetCurrentDifficultyName()} (레벨 {currentDifficultyLevel})");

        // 난이도 변경 이벤트 발생 (C# event)
        OnDifficultyChanged?.Invoke(currentDifficultyLevel);

        // CurrencyManager 배율 업데이트 요청
        if (CurrencyManager_KJG.Instance != null)
        {
            CurrencyManager_KJG.Instance.UpdateMultipliers(currentDifficultyLevel);
        }

        // 글로벌 이벤트 발생 (UI 새로고침, 저장 등)
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RefreshUI);
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RequestSave);
    }

    // ==================== 세이브 / 로드 ====================
    public int GetSaveData() => currentDifficultyLevel;

    public void LoadFromSave(int level)
    {
        currentDifficultyLevel = Mathf.Clamp(level, 0, difficultyNames.Length - 1);
        OnDifficultyChanged?.Invoke(currentDifficultyLevel);   // 로드 후에도 이벤트 발생
    }

    // ==================== 배율 getter ====================
    public float GetCurrentGoldMultiplier()
    {
        return goldMultiplierPerDifficulty[Mathf.Clamp(currentDifficultyLevel, 0, goldMultiplierPerDifficulty.Length - 1)];
    }

    public float GetCurrentExpMultiplier()
    {
        return expMultiplierPerDifficulty[Mathf.Clamp(currentDifficultyLevel, 0, expMultiplierPerDifficulty.Length - 1)];
    }
}