using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager_KJG : MonoBehaviour
{
    public static DifficultyManager_KJG Instance {  get; private set; }

    public int currentDifficultyLevel = 0;

    private readonly string[] difficultyNames =
    {
        "Easy","Nomal","Hard"
    };

    //다음 난이도에 필수 환생한 헌터의 수
    private readonly int[] requiredReincarnatedHunters =
    {
        0,  // 이지
        5,  // 노말
        10  // 하드
    };

    // 난이도에 따라 경험치와 골드 배율 수정은 여기서
    public float[] goldMultiplierPerDifficulty = { 1f, 1.5f, 2.2f};
    public float[] expMultiplierPerDifficulty = { 1f, 1.5f, 1.2f};

    // 몬스터 스탯 배율 (난이도 올라갈수록 더 세짐) currentDifficultyLevel = 0,1,2
    public float monsterHpMultiplier => 1f + (currentDifficultyLevel * 0.8f);
    public float monsterAtkMultiplier => 1f + (currentDifficultyLevel * 0.7f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //현재 난이도 이름 가져오는 함수
    public string GetCurrentDifficultyName()
    {
        if (currentDifficultyLevel < 0 || currentDifficultyLevel >= difficultyNames.Length)
            return "GetCurrentDifficultyName_Bug";

        return difficultyNames[currentDifficultyLevel];
    }

    // 난이도 업 가능 여부 체크 (UI 버튼 활성화용) 가능하면 active = true 아니면 false
    //public bool CanUpgradeDifficulty()
    //{
    //    if (currentDifficultyLevel >= difficultyNames.Length - 1) return false;

    //    int required = requiredReincarnatedHunters[currentDifficultyLevel];
    //    int currentReincarnatedCount = HunterManager_KJG.Instance?.GetReincarnatedHunterCount() ?? 0; 머지 후 헌터매니저와 연결

    //    return currentReincarnatedCount >= required;
    //}

    // 난이도 업그레이드 실행
    public void UpgradeDifficulty()
    {
        // if (!CanUpgradeDifficulty()) return; 머지 후 연결

        currentDifficultyLevel++;

        // 보너스 즉시 적용 (CurrencyManager에도 반영 가능)
        CurrencyManager_KJG.Instance?.UpdateMultipliers();  // 배율 다시 계산(나중에 손볼 코드)

        Debug.Log($"난이도 업그레이드! → {GetCurrentDifficultyName()}");

        // 이벤트 발생 (UI 새로고침, 이펙트, 설명 팝업 등)
        // 이벤트 함수 호출할꺼임 나중에 OnDifficultyChanged?.Invoke(currentDifficultyLevel);EventManager.Publish(new DifficultyUpgradedEvent(currentDifficultyLevel));

        // SoundManager_KJG.Instance?.PlaySFX("difficulty_up"); 나중에 사운드 매니저를 통한 효과음 추가

    }

    // 세이브/로드용
    public int GetSaveData() => currentDifficultyLevel;
    public void LoadFromSave(int level) => currentDifficultyLevel = level;
}
