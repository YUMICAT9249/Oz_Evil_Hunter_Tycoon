using UnityEngine;

// ★ 거래소 기능 (재료 수급 전용)
public class TradeInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public string itemID = "Loot";

    private BuildingLevelComponent_YHJ levelComponent;

    void Awake()
    {
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return true;
    }

    public void Interact(IUnit_YHJ unit)
    {
        // ★ 유닛 → 아이템 제출 요청 (헌터팀 연결)
        EventBus_YHJ.RequestItemFromUnit?.Invoke(unit, itemID);
    }

    void OnEnable()
    {
        EventBus_YHJ.OnItemReceived += OnItemReceived;
    }

    void OnDisable()
    {
        EventBus_YHJ.OnItemReceived -= OnItemReceived;
    }

    private void OnItemReceived(IUnit_YHJ unit, string id, int amount)
    {
        if (unit == null || id != itemID)
            return;

        if (amount <= 0)
        {
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        // ★ 레벨별 매입 가능 소재 체크
        if (levelComponent != null && !levelComponent.CanUseItem(id))
        {
            Debug.Log("[Trade] 현재 레벨에서 매입 불가");
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        MaterialInventory_YHJ.Instance.AddItem(id, amount);

        Debug.Log($"[Trade] {id} +{amount}");

        EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Success);
    }
}