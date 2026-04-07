using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance_YHJ
{
    public string buildingID;

    public BuildingType_YHJ buildingType;

    public Vector2Int origin;
    public Vector2Int size;
    public GameObject instance;
    public List<Vector2Int> occupiedCells;

    public int currentLevel = 1;
    public BuildingLevelData_YHJ levelData;

    // 현재 스탯
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

    // 레벨 업
    public bool TryUpgrade(ref int gold)
    {
        if (levelData == null || levelData.levelStats == null)
            return false;

        if (currentLevel >= levelData.levelStats.Count)
            return false;

        var nextStat = levelData.levelStats[currentLevel];

        if (gold < nextStat.upgradeCost)
            return false;

        gold -= nextStat.upgradeCost;
        currentLevel++;

        return true;
    }

    // 매니저 등록
    public void Register()
    {
        if (BuildingManager_YHJ.Instance != null)
            BuildingManager_YHJ.Instance.RegisterBuilding(this);
    }

    public void Unregister()
    {
        if (BuildingManager_YHJ.Instance != null)
            BuildingManager_YHJ.Instance.UnregisterBuilding(this);
    }
}