using UnityEngine;
using System.Collections.Generic;

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

    // ★ KJG 추가: Save/Load를 위해 필요한 필드 (LevelStat에서 가져오는 값)
    public int capacity;
    public float workSpeed;

    // ★ KJG 추가: 업그레이드 성공 시 다른 시스템(UI, Sound, SaveLoad)이 구독할 수 있는 이벤트
    public event System.Action OnUpgraded;

    public void Initialize(string id, BuildingLevelData_YHJ data, GameObject obj)
    {
        buildingID = id;
        levelData = data;
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

        // ★ KJG 추가: Save/Load를 위해 capacity와 workSpeed 저장
        capacity = stat.capacity;
        workSpeed = stat.workSpeed;
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
    public bool TryUpgrade()
    {
        if (levelData == null || CurrentStat == null) return false;
        if (currentLevel >= levelData.levelStats.Count)
            return false;

        var nextStat = levelData.levelStats[currentLevel];

        // ★ KJG 수정: CurrencyManager를 통해 Gold 소비 처리 (ref int gold 방식 제거)
        if (!Manager_KJG.Currency.SpendGold(nextStat.upgradeCost))
        {
            Debug.LogWarning($"[BuildingInstance] Gold 부족! 필요: {nextStat.upgradeCost}");
            return false;
        }

        currentLevel++;
        ApplyLevel();

        // ★ KJG 추가: 업그레이드 성공 시 이벤트 발생
        OnUpgraded?.Invoke();

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