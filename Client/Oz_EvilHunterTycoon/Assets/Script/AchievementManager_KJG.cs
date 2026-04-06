using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 업적 관리 매니저
/// 
/// 특징:
/// - Manager_KJG.Achievement 형태로만 접근 가능
/// - 직접 .Instance 호출 완전 제거
/// - 이벤트는 Manager_KJG.Event를 사용
/// - 코드가 매우 읽기 쉽고 실무 수준으로 정리됨
/// </summary>
public class AchievementManager_KJG : BaseManager_KJG<AchievementManager_KJG>
{
    [Header("업적 목록")]
    [SerializeField] private List<Achievement> achievements = new List<Achievement>();

    // ==================== 업적 데이터 구조 ====================
    [System.Serializable]
    public class Achievement
    {
        public string id;                    // 고유 키 (예: "kill_100_enemies")
        public string title;                 // 업적 이름
        public string description;           // 설명
        public int current;                  // 현재 진행도
        public int target;                   // 목표치
        public bool isUnlocked;              // 달성 여부
        public bool isSecret;                // 비밀 업적 여부
        public DateTime? unlockTime;         // 달성 시간
        public Sprite icon;                  // 아이콘
        public string unlockMessage;         // 달성 메시지

        public float Progress => target > 0 ? (float)current / target : 0f;
        public bool IsCompleted => current >= target && !isUnlocked;
    }

    // ==================== 이벤트 ====================
    public event Action<Achievement> OnAchievementUnlocked;
    public event Action<Achievement> OnAchievementProgress;

    protected override void Awake()
    {
        base.Awake();
        InitializeDefaultAchievements();
        Debug.Log("✅ [AchievementManager_KJG] 업적 시스템 초기화 완료");
    }

    private void InitializeDefaultAchievements()
    {
        // 필요하면 여기서 기본 업적을 코드로 추가 (Inspector에서 추가하는 것을 추천)
    }

    // ==================== 업적 진행 & 달성 ====================
    public void AddProgress(string achievementId, int amount = 1)
    {
        Achievement ach = GetAchievementById(achievementId);
        if (ach == null || ach.isUnlocked) return;

        ach.current += amount;
        ach.current = Mathf.Min(ach.current, ach.target);

        OnAchievementProgress?.Invoke(ach);

        if (ach.current >= ach.target && !ach.isUnlocked)
            UnlockAchievement(ach);
    }

    private void UnlockAchievement(Achievement ach)
    {
        ach.isUnlocked = true;
        ach.unlockTime = DateTime.Now;

        Debug.Log($"🏆 업적 달성! [{ach.title}] - {ach.description}");

        OnAchievementUnlocked?.Invoke(ach);

        // 글로벌 이벤트 발생
        Manager_KJG.Event.RefreshUI();
        Manager_KJG.SaveLoad.GameSave();   // 저장 요청
    }

    // ==================== 조회 메서드 ====================
    public Achievement GetAchievementById(string id)
        => achievements.Find(a => a.id == id);

    public List<Achievement> GetAllAchievements()
        => new List<Achievement>(achievements);

    public List<Achievement> GetSaveData()
        => achievements;

    // ==================== 세이브/로드 ====================
    public void LoadFromSave(List<Achievement> savedAchievements)
    {
        if (savedAchievements == null) return;

        achievements = savedAchievements;

        foreach (var ach in achievements)
        {
            if (ach.isUnlocked)
                OnAchievementUnlocked?.Invoke(ach);
        }
    }
}