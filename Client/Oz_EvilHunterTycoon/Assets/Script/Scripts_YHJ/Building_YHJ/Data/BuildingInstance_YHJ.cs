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

    public int capacity;
    public float workSpeed;
    private GameObject owner;

    public void Initialize(string id, BuildingLevelData_YHJ data, GameObject obj)
    {
        buildingID = id;
        levelData = data;
        owner = obj;

        currentLevel = 1;

        ApplyLevel();
    }

    public void ApplyLevel()
    {
        if (levelData == null)
        {
            Debug.LogError($"[BuildingInstance] LevelData 없음: {buildingID}");
            return;
        }

        if (currentLevel <= 0 || currentLevel > levelData.levelStats.Count)
        {
            Debug.LogError($"[BuildingInstance] 레벨 범위 이상: {currentLevel}");
            return;
        }

        LevelStat stat = levelData.levelStats[currentLevel - 1];

        // 🔥 공통 적용
        capacity = stat.capacity;
        workSpeed = stat.workSpeed;

        // 필요하면 계속 추가
        // healAmount = stat.healAmount;
        // reviveDelay = stat.reviveDelay;
    }
    public void Upgrade()
    {
        if (levelData == null) return;

        if (currentLevel >= levelData.MaxLevel)
        {
            Debug.Log("최대 레벨");
            return;
        }

        currentLevel++;

        ApplyLevel();
    }

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