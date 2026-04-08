using UnityEngine;

// 치료소 기능
public class HealInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public string itemID = "Bandage";
    public float healAmount = 20f;

    private BuildingInventory_YHJ inventory;
    private BuildingQueue_YHJ queue;
    private BuildingLevelComponent_YHJ levelComponent;

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
        queue = GetComponent<BuildingQueue_YHJ>();
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
        return unit.CurrentHP < unit.MaxHP;
    }

    public void Interact(IUnit_YHJ unit)
    {
        if (inventory.TryConsume(itemID, 1))
        {
            Debug.Log("[Heal] 즉시 치료");
            unit.Heal(GetHealAmount());

            EventBus_YHJ.OnInteractionResult?.Invoke
            (
                unit,
                InteractionResult_YHJ.Success
            );
        }
        else
        {
            Debug.Log("[Heal] 재고 없음 → 큐");

            queue.Enqueue(unit);

            EventBus_YHJ.OnInteractionResult?.Invoke
            (
                unit,
                InteractionResult_YHJ.Queued
            );
        }
    }

    // ⭐ 핵심 처리 함수
    private void OnProcessUnit(IUnit_YHJ unit, GameObject building)
    {
        if (building != gameObject)
            return;

        Debug.Log("[Heal] 큐 처리");

        // ❗ 여기 핵심: 실패해도 다시 큐 안 넣음
        if (!inventory.TryConsume(itemID, 1))
        {
            Debug.Log("[Heal] 재고 없음 → 대기 유지");
            return;
        }

        Debug.Log("[Heal] 치료 성공");
        unit.Heal(GetHealAmount());

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
    private float GetHealAmount()
    {
        if (levelComponent == null)
            return healAmount;

        if (levelComponent.CurrentStat == null)
            return healAmount;

        return levelComponent.CurrentStat.healAmount;
    }
}