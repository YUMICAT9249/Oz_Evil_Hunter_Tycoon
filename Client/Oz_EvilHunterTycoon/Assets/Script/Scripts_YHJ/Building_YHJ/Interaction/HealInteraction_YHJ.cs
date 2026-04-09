using UnityEngine;

// ★ 치료소 기능
public class HealInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingInventory_YHJ inventory;
    private BuildingQueue_YHJ queue;
    private BuildingLevelComponent_YHJ levelComponent;

    public string bandageID = "Bandage";

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
        if (unit == null || unit.IsDead)
            return false;

        float triggerHpPercent = 0.5f;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            if (levelComponent.CurrentStat.autoHealHpPercent > 0f)
                triggerHpPercent = levelComponent.CurrentStat.autoHealHpPercent;
        }

        return unit.CurrentHP <= unit.MaxHP * triggerHpPercent;
    }

    public void Interact(IUnit_YHJ unit)
    {
        if (unit == null)
            return;

        // ★ 헌터 자동 이동 후 치료소 도착 시 대기열 등록
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

        if (unit == null || unit.IsDead)
            return;

        if (!inventory.HasItem(bandageID, 1))
        {
            TryMakeBandage();
        }

        if (!inventory.HasItem(bandageID, 1))
        {
            Debug.Log("[Heal] 붕대 없음");
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        // ★ 헌터 골드 차감 처리 (헌터팀 / 경제팀 연결)
        // if (!unit.TrySpendGold(...)) return;

        inventory.RemoveItem(bandageID, 1);

        float heal = 30f;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            heal = levelComponent.CurrentStat.healAmount;
        }

        unit.Heal(heal);

        // ★ 아직 체력이 부족하면 다시 대기열
        if (unit.CurrentHP < unit.MaxHP)
        {
            queue.Enqueue(unit);
        }

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }

    private bool TryMakeBandage()
    {
        if (levelComponent != null && !levelComponent.CanUseItem(bandageID))
            return false;

        if (!MaterialInventory_YHJ.Instance.HasItem("Cloth", 2))
            return false;

        MaterialInventory_YHJ.Instance.RemoveItem("Cloth", 2);

        inventory.AddItem(bandageID, 1);

        // ★ UI 제작 완료 연결
        // EventBus_YHJ.OnCraftCompleted?.Invoke(bandageID, 1);

        return true;
    }
}