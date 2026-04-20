using UnityEngine;

// ★ 장비점 기능
public class ArmorShopInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;
    private BuildingInventory_YHJ inventory;
    private BuildingLevelComponent_YHJ levelComponent;

    public string equipmentID = "Equipment";

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
        inventory = GetComponent<BuildingInventory_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

    void OnEnable()
    {
        EventBus_YHJ.RequestProcessUnit += OnProcessUnit;
    }

    void OnDisable()
    {
        EventBus_YHJ.RequestProcessUnit -= OnProcessUnit;
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return unit != null && !unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        queue.Enqueue(unit);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }

    private void OnProcessUnit(IUnit_YHJ unit, GameObject building)
    {
        if (building != gameObject)
            return;

        if (!inventory.HasItem(equipmentID, 1))
        {
            TryMakeEquipment();
        }

        if (!inventory.HasItem(equipmentID, 1))
        {
            Debug.Log("[ArmorShop] 재고 없음");
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        // ★ 플레이어 조작 판매형이므로 실제 판매/장착/UI는 협업 구간
        Manager_KJG.Audio?.PlaySFX("CD01042");
        EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Success);
    }

    private bool TryMakeEquipment()
    {
        if (levelComponent != null && !levelComponent.CanUseItem(equipmentID))
            return false;

        // ★ 장비 재료 규칙은 장비 데이터/UI팀과 연결 필요
        if (!MaterialInventory_YHJ.Instance.HasItem("Iron", 2))
            return false;

        MaterialInventory_YHJ.Instance.RemoveItem("Iron", 2);

        inventory.AddItem(equipmentID, 1);

        return true;
    }
}