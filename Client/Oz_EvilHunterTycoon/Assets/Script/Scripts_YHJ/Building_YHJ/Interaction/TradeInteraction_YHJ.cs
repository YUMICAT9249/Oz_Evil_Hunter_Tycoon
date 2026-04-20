using UnityEngine;

// ★ 거래소 기능 (재료 수급 전용)
public class TradeInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    [Header("직접 판매 기본값")]
    [SerializeField] private string itemID = "Loot";

    [Header("요청 매입 상태")]
    [SerializeField] private string requestedItemID = string.Empty;
    [SerializeField] private int requestedAmount = 0;

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
        if (unit == null)
            return;

        // ★ UI에서 요청 매입이 걸려 있으면 "필요 수량만큼 판매" 이벤트를 우선 보냅니다.
        if (HasActivePurchaseRequest())
        {
            EventBus_YHJ.RequestSellItem?.Invoke(unit, requestedItemID, requestedAmount);
            return;
        }

        // ★ 유닛 → 아이템 제출 요청 (레거시 직접 판매 흐름)
        // UI 담당:
        // - 직접 판매 버튼에서 SetDirectSellItem(itemID)로 판매 대상 지정
        // - 이후 헌터를 거래소로 보내면 이 흐름을 사용합니다.
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

    // ★ UI 담당:
    // 거래소에서 "이 재료를 팔아라" 같은 직접 판매 명령을 만들 때 사용합니다.
    // 이후 헌터가 거래소와 상호작용하면 RequestItemFromUnit(unit, itemID)가 호출됩니다.
    public void SetDirectSellItem(string directSellItemID)
    {
        itemID = string.IsNullOrEmpty(directSellItemID) ? string.Empty : directSellItemID;
    }

    // ★ UI 담당:
    // 거래소 부족 재료를 채우기 위한 "요청 매입" 등록 진입점입니다.
    // 예) SetPurchaseRequest("CopperOre", 10)
    // 이후 헌터가 거래소에 도착하면 RequestSellItem(unit, "CopperOre", 10)이 호출됩니다.
    public bool SetPurchaseRequest(string targetItemID, int amount)
    {
        if (string.IsNullOrEmpty(targetItemID) || amount <= 0)
            return false;

        if (levelComponent != null && !levelComponent.CanUseItem(targetItemID))
            return false;

        requestedItemID = targetItemID;
        requestedAmount = amount;
        NotifyTradeRequestChanged();
        return true;
    }

    // ★ UI 담당:
    // 요청 매입 취소 버튼에 연결할 때 사용합니다.
    public void ClearPurchaseRequest()
    {
        requestedItemID = string.Empty;
        requestedAmount = 0;
        NotifyTradeRequestChanged();
    }

    // ★ UI 담당:
    // 거래소 패널 갱신 시 현재 요청 상태를 읽을 때 사용합니다.
    public bool HasActivePurchaseRequest()
    {
        return !string.IsNullOrEmpty(requestedItemID) && requestedAmount > 0;
    }

    public string GetRequestedItemID()
    {
        return requestedItemID;
    }

    public int GetRequestedAmount()
    {
        return requestedAmount;
    }

    public string GetDirectSellItemID()
    {
        return itemID;
    }

    private void OnItemReceived(IUnit_YHJ unit, string id, int amount)
    {
        if (unit == null || string.IsNullOrEmpty(id))
            return;

        if (amount <= 0)
        {
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        bool isRequestedTrade = HasActivePurchaseRequest() && id == requestedItemID;
        bool isDirectTrade = id == itemID;

        if (!isRequestedTrade && !isDirectTrade)
            return;

        // ★ 레벨별 매입 가능 소재 체크
        if (levelComponent != null && !levelComponent.CanUseItem(id))
        {
            Debug.Log("[Trade] 현재 레벨에서 매입 불가");
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        MaterialInventory_YHJ.Instance.AddItem(id, amount);

        Debug.Log($"[Trade] {id} +{amount}");

        if (isRequestedTrade)
        {
            requestedAmount = Mathf.Max(0, requestedAmount - amount);

            if (requestedAmount <= 0)
                requestedItemID = string.Empty;

            NotifyTradeRequestChanged();
        }

        EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Success);
    }

    private void NotifyTradeRequestChanged()
    {
        EventBus_YHJ.OnTradeRequestChanged?.Invoke(gameObject, requestedItemID, requestedAmount);
    }
}