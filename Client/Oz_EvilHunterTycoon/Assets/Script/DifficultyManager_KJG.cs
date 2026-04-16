using UnityEngine;

/// <summary>
/// 난이도 관리 매니저
///
/// 특징:
/// - Manager_KJG.Difficulty 형태로만 접근
/// - 난이도 업그레이드, 배율 관리 등을 Manager_KJG를 통해 사용
/// </summary>
public class DifficultyManager_KJG : BaseManager_KJG<DifficultyManager_KJG>
{
    [Header("현재 난이도")]
    public int currentDifficultyLevel = 0;

    private readonly string[] difficultyNames = { "Easy", "Normal", "Hard" };

    public float[] goldMultiplierPerDifficulty = { 1f, 1.5f, 2.2f };
    public float[] expMultiplierPerDifficulty = { 1f, 1.5f, 1.2f };

    public float monsterHpMultiplier => 1f + (currentDifficultyLevel * 0.8f);
    public float monsterAtkMultiplier => 1f + (currentDifficultyLevel * 0.7f);

    public event System.Action<int> OnDifficultyChanged;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [DifficultyManager_KJG] 난이도 시스템 초기화 완료");
    }

    public string GetCurrentDifficultyName()
    {
        if (currentDifficultyLevel < 0 || currentDifficultyLevel >= difficultyNames.Length)
            return "Unknown";
        return difficultyNames[currentDifficultyLevel];
    }

    public bool CanUpgradeDifficulty()
    {
        if (currentDifficultyLevel >= difficultyNames.Length - 1)
            return false;
        return true; // 테스트용 (나중에 HunterManager와 연동 가능)
    }

    public void UpgradeDifficulty()
    {
        if (!CanUpgradeDifficulty()) return;

        currentDifficultyLevel++;
        Debug.Log($"난이도 업그레이드 완료 → {GetCurrentDifficultyName()}");

        OnDifficultyChanged?.Invoke(currentDifficultyLevel);

        // ★ KJG 수정: CurrencyManager의 UpdateMultipliers()는 인자를 받지 않음
        // 내부에서 DifficultyManager를 조회해서 배율을 자동 업데이트하도록 호출
        if (Manager_KJG.Currency != null)
            Manager_KJG.Currency.UpdateMultipliers();

        Manager_KJG.Event.RefreshUI();
        Manager_KJG.SaveLoad.GameSave();
    }

    public float GetCurrentGoldMultiplier()
    {
        int index = Mathf.Clamp(currentDifficultyLevel, 0, goldMultiplierPerDifficulty.Length - 1);
        return goldMultiplierPerDifficulty[index];
    }

    public float GetCurrentExpMultiplier()
    {
        int index = Mathf.Clamp(currentDifficultyLevel, 0, expMultiplierPerDifficulty.Length - 1);
        return expMultiplierPerDifficulty[index];
    }

    // ==================== 세이브/로드 ====================
    public int GetSaveData() => currentDifficultyLevel;

    public void LoadFromSave(int level)
    {
        currentDifficultyLevel = Mathf.Clamp(level, 0, difficultyNames.Length - 1);
        OnDifficultyChanged?.Invoke(currentDifficultyLevel);
    }
}