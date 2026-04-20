using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// BuildingManager_YHJ
///
/// BuildingInstance_YHJ와 연동되어 건물을 중앙에서 관리합니다.
/// DropItemPickup_KJG에서 재료를 소비하고, 나중에 Building 업그레이드도 여기서 처리합니다.
/// </summary>
public class BuildingManager_YHJ : BaseManager_KJG<BuildingManager_YHJ>
{
    private List<BuildingInstance_YHJ> buildings = new List<BuildingInstance_YHJ>();

    // public getter 추가 (실무에서 가장 추천하는 방식)
    public List<BuildingInstance_YHJ> Buildings => buildings;

    // ★ KJG 추가: MaterialInventory 참조 (드랍된 재료를 실제로 소비하기 위함)
    private MaterialInventory_YHJ _materialInventory;

    protected override void Awake()
    {
        base.Awake();

        // ★ KJG 수정: MaterialInventory를 안전하게 찾기
        // 이유: Bootstrapper에서 매니저들이 깨어나는 순서가 불확실하기 때문에
        // Instance가 null일 수 있으므로 FindObjectOfType으로 보완
        if (MaterialInventory_YHJ.Instance != null)
        {
            _materialInventory = MaterialInventory_YHJ.Instance;
        }
        else
        {
            _materialInventory = FindObjectOfType<MaterialInventory_YHJ>(true);
        }

        if (_materialInventory == null)
            Debug.LogError("[BuildingManager] MaterialInventory_YHJ를 찾을 수 없습니다!");
        else
            Debug.Log("✅ [BuildingManager] MaterialInventory_YHJ 연결 완료");
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
        // ★ KJG 수정: 실제 MaterialInventory에서 재료 소비 처리
        if (_materialInventory == null)
        {
            Debug.LogError("[BuildingManager] MaterialInventory가 없습니다.");
            return false;
        }

        string itemID = itemType.ToString();
        bool success = _materialInventory.RemoveItem(itemID, amount);

        if (success)
            Debug.Log($"[BuildingManager] {itemType} {amount}개 소비 성공");
        else
            Debug.LogWarning($"[BuildingManager] {itemType} {amount}개 부족");

        return success;
    }
}