using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelStat
{
    public int level;

    // ★ 공통
    public int capacity;
    public float workSpeed;
    public int upgradeCost;

    // ★ 제작 건물 공통 : 현재 레벨에서 사용/제작 가능한 아이템
    public List<string> unlockItemIDs = new List<string>();

    // ★ 치료소
    public float healAmount;
    public float autoHealHpPercent;

    // ★ 성소 - 부활
    public int reviveGoldCost;
    public float reviveDelay;

    // ★ 성소 - 스킬
    public bool canUseMainSkill;
    public bool canUseSubSkill;

    // ★ 수련장
    public int trainingGoldCost;
    public float trainingDuration;

    // ★ 수련장 / 필드 접근용
    public int requiredRebirthCountMin;
    public int requiredRebirthCountMax;

    // ★ 지하던전용은 이번 제외지만 데이터칸은 남겨둠
    public int minDungeonFloor;
    public int maxDungeonFloor;
}
[CreateAssetMenu(menuName = "Building/LevelData")]
[System.Serializable]
public class BuildingLevelData_YHJ
{
    public int MaxLevel => levelStats.Count;
    public List<LevelStat> levelStats = new List<LevelStat>();
}