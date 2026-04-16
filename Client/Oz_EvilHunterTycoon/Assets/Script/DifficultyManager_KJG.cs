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

        // HunterManager와 연동은 나중에
        return true; // 테스트용
    }

    public void UpgradeDifficulty()
    {
        if (!CanUpgradeDifficulty()) return;

        currentDifficultyLevel++;

        Debug.Log($"난이도 업그레이드 완료 → {GetCurrentDifficultyName()}");

        OnDifficultyChanged?.Invoke(currentDifficultyLevel);

        // ★ YHJ: CurrencyManager_KJG.UpdateMultipliers는 현재 인자를 받지 않으므로 호출 형식을 맞춰 컴파일 에러를 수정
        // ★ YHJ: 난이도 상승 직후 골드/경험치 배율이 정상 반영되도록 배율 갱신 기능을 유지
        if (Manager_KJG.Currency != null)
            Manager_KJG.Currency.UpdateMultipliers();

        // ★ YHJ: 매니저 초기화 순서에 따라 null일 수 있어 임시 런타임 에러를 막기 위해 안전 검사 추가
        if (Manager_KJG.Event != null)
            Manager_KJG.Event.RefreshUI();

        // ★ YHJ: 세이브 매니저가 준비된 경우에만 저장해 null 참조 에러를 방지
        if (Manager_KJG.SaveLoad != null)
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
