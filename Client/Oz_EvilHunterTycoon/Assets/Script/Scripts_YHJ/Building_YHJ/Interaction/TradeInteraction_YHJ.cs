using UnityEngine;

// ★ 거래소 기능 (아이템 → 골드)
public class TradeInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public string itemID = "Loot";

    public bool CanInteract(IUnit_YHJ unit)
    {
        // TODO: 헌터가 해당 아이템 가지고 있는지
        return true;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("거래 요청");

        // ★ 헌터에게 아이템 요청 (이건 헌터 담당)
        EventBus_YHJ.RequestItemFromUnit?.Invoke(unit, itemID);
    }
}