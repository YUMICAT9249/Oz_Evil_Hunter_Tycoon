using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [KJG 실무 아키텍처] AchievementManager_KJG
///
/// 역할:
/// - 업적 조건 체크 및 해제 관리
/// - SaveLoadManager_KJG와 연동 (UnlockedAchievements 저장/로드)
/// - 몬스터 처치, 건물 업그레이드, 헌터 레벨 등 원작 스타일 업적 지원
/// - 팀원이 쉽게 업적을 추가할 수 있게 설계
/// </summary>
public class AchievementManager_KJG : BaseManager_KJG<AchievementManager_KJG>
{
    // 해제된 업적 목록 (Save/Load에서 사용)
    private HashSet<string> unlockedAchievements = new HashSet<string>();

    [Header("업적 목록 (팀원이 쉽게 추가/수정 가능)")]
    public List<AchievementData> achievementList = new List<AchievementData>();

    [System.Serializable]
    public class AchievementData
    {
        public string id;               // 업적 고유 ID (예: "Kill100Monsters")
        public string title;            // 업적 이름 (예: "초보 사냥꾼")
        public string description;      // 업적 설명
        public bool isUnlocked;         // 현재 해제 여부
    }

    /// <summary>
    /// 업적 해제 체크
    /// </summary>
    public void CheckAndUnlock(string achievementId)
    {
        if (unlockedAchievements.Contains(achievementId)) return;

        var achievement = achievementList.Find(a => a.id == achievementId);
        if (achievement != null)
        {
            achievement.isUnlocked = true;
            unlockedAchievements.Add(achievementId);
            Debug.Log($"[Achievement] 업적 해제! → {achievement.title}");
        }
    }

    // ==================== 업적 조건 예시 메서드 ====================
    public void OnMonsterKilled() => CheckAndUnlock("Kill100Monsters");
    public void OnBuildingUpgraded(string buildingID) => CheckAndUnlock("UpgradeBuilding");
    public void OnHunterLevelUp(int level)
    {
        if (level >= 10) CheckAndUnlock("HunterLevel10");
    }

    // ==================== Save/Load 연동 ====================
    public void SaveAchievements(SaveLoadManager_KJG.SaveData saveData)
    {
        saveData.UnlockedAchievements.Clear();
        saveData.UnlockedAchievements.AddRange(unlockedAchievements);
    }

    public void LoadAchievements(SaveLoadManager_KJG.SaveData saveData)
    {
        unlockedAchievements.Clear();
        foreach (var id in saveData.UnlockedAchievements)
        {
            unlockedAchievements.Add(id);
            var achievement = achievementList.Find(a => a.id == id);
            if (achievement != null) achievement.isUnlocked = true;
        }
    }
}