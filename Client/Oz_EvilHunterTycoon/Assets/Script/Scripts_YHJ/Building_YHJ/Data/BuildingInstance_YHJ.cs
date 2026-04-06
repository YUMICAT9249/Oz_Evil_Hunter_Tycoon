using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance_YHJ
{
    public string buildingID;
    public Vector2Int origin;
    public Vector2Int size;
    public GameObject instance;
    public List<Vector2Int> occupiedCells;

    public int currentLevel = 1;
    public BuildingLevelData_YHJ levelData;

    public LevelStat CurrentStat
    {
        get
        {
            if (levelData == null || levelData.levelStats == null)
                return null;

            int index = currentLevel - 1;

            if (index < 0 || index >= levelData.levelStats.Count)
                return null;

            return levelData.levelStats[index];
        }
    }

    public bool TryUpgrade(ref int gold)
    {
        if (currentLevel >= levelData.levelStats.Count)
            return false;

        var nextStat = levelData.levelStats[currentLevel];

        if (gold < nextStat.upgradeCost)
            return false;

        gold -= nextStat.upgradeCost;

        currentLevel++;

        return true;
    }
}

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
