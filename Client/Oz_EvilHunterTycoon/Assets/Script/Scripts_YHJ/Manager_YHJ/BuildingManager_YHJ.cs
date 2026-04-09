using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingManager_YHJ : BaseManager_KJG<BuildingManager_YHJ>
{
    private List<BuildingInstance_YHJ> buildings = new List<BuildingInstance_YHJ>();

    protected override void Awake()
    {
        base.Awake();
    }

    // 등록
    public void RegisterBuilding(BuildingInstance_YHJ building)
    {
        if (!buildings.Contains(building))
            buildings.Add(building);
    }

    // 제거
    public void UnregisterBuilding(BuildingInstance_YHJ building)
    {
        if (buildings.Contains(building))
            buildings.Remove(building);
    }

    // 타입별 조회
    public List<BuildingInstance_YHJ> GetByType(BuildingType_YHJ type)
    {
        return buildings.Where(b => b.buildingType == type).ToList();
    }

    // 가장 가까운 건물
    public BuildingInstance_YHJ GetNearest(BuildingType_YHJ type, Vector3 pos)
    {
        return buildings
            .Where(b => b.buildingType == type)
            .OrderBy(b => Vector3.Distance(pos, b.instance.transform.position))
            .FirstOrDefault();
    }
}