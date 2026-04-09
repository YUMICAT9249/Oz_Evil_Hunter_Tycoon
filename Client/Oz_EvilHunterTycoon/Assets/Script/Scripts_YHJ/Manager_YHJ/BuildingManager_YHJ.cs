using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// [KJG 아키텍처] BuildingManager_YHJ
///
/// BuildingInstance_YHJ와 연동되어 건물을 중앙에서 관리합니다.
/// </summary>
public class BuildingManager_YHJ : BaseManager_KJG<BuildingManager_YHJ>
{
    private List<BuildingInstance_YHJ> buildings = new List<BuildingInstance_YHJ>();

    // public getter 추가 (실무에서 가장 추천하는 방식)
    public List<BuildingInstance_YHJ> Buildings => buildings;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
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

    /// <summary>
    /// 드랍된 재료를 소비합니다 (Building 업그레이드 비용으로 사용)
    /// DropItemPickup_KJG에서 호출됩니다.
    /// </summary>
    public bool ConsumeMaterial(DropItemType itemType, int amount)
    {
        Debug.Log($"[BuildingManager_YHJ] {itemType} {amount}개 소비 (업그레이드 비용으로 사용)");

        // TODO: 실제 재료 인벤토리나 저장 시스템과 연결
        // 예: inventory.RemoveMaterial(itemType, amount);

        return true; // 소비 성공
    }
}