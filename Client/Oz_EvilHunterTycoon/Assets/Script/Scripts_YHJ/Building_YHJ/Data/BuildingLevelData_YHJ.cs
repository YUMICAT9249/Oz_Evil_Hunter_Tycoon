using System.Collections.Generic;

[System.Serializable]
public class LevelStat
{
    public int level;
    public int capacity;
    public float workSpeed;
    public int upgradeCost;
}

[System.Serializable]
public class BuildingLevelData_YHJ
{
    public int MaxLevel => levelStats.Count;
    public List<LevelStat> levelStats;
}