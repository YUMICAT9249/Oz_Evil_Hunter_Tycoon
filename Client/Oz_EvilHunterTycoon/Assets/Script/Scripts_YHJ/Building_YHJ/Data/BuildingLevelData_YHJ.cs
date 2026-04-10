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

    // ★ 제작 건물 공통
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

    // ★ 수련장 / 필드 접근
    public int requiredRebirthCountMin;
    public int requiredRebirthCountMax;

    // ★ 던전
    public int minDungeonFloor;
    public int maxDungeonFloor;
}

[CreateAssetMenu(menuName = "Building/LevelData")]
public class BuildingLevelData_YHJ : ScriptableObject
{
    public List<LevelStat> levelStats = new List<LevelStat>();

    public int MaxLevel => levelStats.Count;
}