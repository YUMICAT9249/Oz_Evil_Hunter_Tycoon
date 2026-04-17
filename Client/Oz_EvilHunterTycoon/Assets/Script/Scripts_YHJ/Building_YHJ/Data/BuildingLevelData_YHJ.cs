using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelStat
{
    public int level;

    public int capacity;
    public float workSpeed;
    public int upgradeCost;

    public List<string> unlockItemIDs = new List<string>();
    public List<ItemData_YHJ> unlockItems = new List<ItemData_YHJ>();

    public float autoHealHpPercent;

    public int reviveGoldCost;
    public float reviveDelay;

    public bool canUseMainSkill;
    public bool canUseSubSkill;
    public int mainSkillMaxLevel;
    public int subSkillMaxLevel;
    public int mainSkillUpgradeCost;
    public int subSkillUpgradeCost;

    public int trainingGoldCost;
    public float trainingDuration;

    public int requiredRebirthCountMin;
    public int requiredRebirthCountMax;

    public int minDungeonFloor;
    public int maxDungeonFloor;
}

[CreateAssetMenu(menuName = "Building/LevelData")]
public class BuildingLevelData_YHJ : ScriptableObject
{
    public List<LevelStat> levelStats = new List<LevelStat>();

    public int MaxLevel => levelStats.Count;
}
