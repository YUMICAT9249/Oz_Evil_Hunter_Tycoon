using UnityEngine;

// ★ 거래소 기능 (아이템 → 골드)
public class TradeInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public string itemID = "Loot";
    public int price = 5;

    public bool CanInteract(IUnit_YHJ unit)
    {
        return true;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("[Trade] 거래 요청");

        // 아이템 요청
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
            Debug.Log("[Trade] 아이템 없음 → 거래 실패");

            EventBus_YHJ.OnInteractionResult?.Invoke
            (
                unit,
                InteractionResult_YHJ.Fail
            );

            return;
        }

        Debug.Log("[Trade] 아이템 수령 → 골드 지급");

        // TODO: 골드 지급
        // unit.AddGold(price * amount);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
}