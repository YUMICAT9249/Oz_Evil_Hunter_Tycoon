using UnityEngine;

public class PotionShopInteraction_YHJ : MonoBehaviour
{
    public string itemID = "Potion";

    private BuildingInventory_YHJ inventory;

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
    }

    void OnEnable()
    {
        EventBus_YHJ.RequestBuyItem += OnRequestBuyItem;
    }

    void OnDisable()
    {
        EventBus_YHJ.RequestBuyItem -= OnRequestBuyItem;
    }

    private void OnRequestBuyItem(IUnit_YHJ unit, string id)
    {
        if (id != itemID)
            return;

        if (!inventory.TryConsume(itemID, 1))
        {
            Debug.Log("[PotionShop] 재고 없음");

            EventBus_YHJ.OnInteractionResult?.Invoke
            (
                unit,
                InteractionResult_YHJ.Fail
            );

            return;
        }

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );

        Debug.Log("[PotionShop] 포션 지급");

        // unit.AddItem(itemID);
    }
}
