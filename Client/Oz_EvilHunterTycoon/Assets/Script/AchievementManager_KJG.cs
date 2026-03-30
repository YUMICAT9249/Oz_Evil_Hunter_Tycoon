using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager_KJG : MonoBehaviour
{
    public static AchievementManager_KJG Instance { get; private set; }

    [System.Serializable]
    public class Achievement
    {
        public string id;                    // 고유 키 (예: "kill_100_enemies")
        public string title;                 // 업적 이름
        public string description;           // 업적 설명
        public int current;                  // 현재 진행도
        public int target;                   // 목표치
        public bool isUnlocked;              // 달성 여부
        public bool isSecret;                // 비밀 업적 여부
        public DateTime? unlockTime;         // 달성 시간
        public Sprite icon;                  // 업적 아이콘
        public string unlockMessage;         // 달성 시 표시할 메시지

        // 진행률 계산 프로퍼티
        public float Progress => target > 0 ? (float)current / target : 0f;
        public bool IsCompleted => current >= target && !isUnlocked;
    }

    [Header("업적 목록")]
    [SerializeField] private List<Achievement> achievements = new List<Achievement>();

    // ==================== C# Events ====================
    public event Action<Achievement> OnAchievementUnlocked;   // 업적 달성 시
    public event Action<Achievement> OnAchievementProgress;   // 진행도 변경 시

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("✅ AchievementManager_KJG 초기화 완료");

        // 필요 시 초기 업적 데이터 로드 또는 초기화
        InitializeDefaultAchievements();
    }

    // ==================== 업적 진행 & 달성 ====================
    public void AddProgress(string achievementId, int amount = 1)
    {
        Achievement ach = GetAchievementById(achievementId);
        if (ach == null)
        {
            Debug.LogWarning($"Achievement ID '{achievementId}'를 찾을 수 없습니다.");
            return;
        }

        if (ach.isUnlocked) return; // 이미 달성한 업적은 진행 안 함

        ach.current += amount;
        ach.current = Mathf.Min(ach.current, ach.target); // 목표치 초과 방지

        OnAchievementProgress?.Invoke(ach);

        // 목표 달성 체크
        if (ach.current >= ach.target && !ach.isUnlocked)
        {
            UnlockAchievement(ach);
        }
    }

    private void UnlockAchievement(Achievement ach)
    {
        ach.isUnlocked = true;
        ach.unlockTime = DateTime.Now;

        Debug.Log($"🏆 업적 달성! [{ach.title}] - {ach.description}");

        OnAchievementUnlocked?.Invoke(ach);

        // 글로벌 이벤트 연동
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RefreshUI);
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RequestSave);
    }

    // ==================== 조회 메서드 ====================
    public Achievement GetAchievementById(string id)
    {
        return achievements.Find(a => a.id == id);
    }

    public List<Achievement> GetAllAchievements() => new List<Achievement>(achievements);

    public List<Achievement> GetUnlockedAchievements()
    {
        return achievements.FindAll(a => a.isUnlocked);
    }

    // ==================== 초기 업적 데이터 (필요시 확장) ====================
    private void InitializeDefaultAchievements()
    {
        // 여기서 기본 업적을 코드로 추가하거나, ScriptableObject로 관리하는 것도 좋습니다.
        // 지금은 Inspector에서 직접 추가하는 것을 추천합니다.
    }

    // ==================== 세이브 / 로드용 ====================
    public List<Achievement> GetSaveData() => achievements;

    public void LoadFromSave(List<Achievement> savedAchievements)
    {
        if (savedAchievements == null) return;

        achievements = savedAchievements;

        // 로드 후 달성된 업적은 이벤트 발생
        foreach (var ach in achievements)
        {
            if (ach.isUnlocked)
            {
                OnAchievementUnlocked?.Invoke(ach);
            }
        }
    }
}