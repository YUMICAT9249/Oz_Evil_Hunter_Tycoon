using System.Collections.Generic;
using UnityEngine;

public class PotionShopInteraction_YHJ : MonoBehaviour
{
    public string itemID = "Potion";
    public List<string> unlockPotionIDs;
    private BuildingLevelComponent_YHJ levelComponent;

    private BuildingInventory_YHJ inventory;

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
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

        if (!CanSellPotion(id))
        {
            Debug.Log("[PotionShop] 레벨 부족");

            EventBus_YHJ.OnInteractionResult?.Invoke
            (
                unit,
                InteractionResult_YHJ.Fail
            );

            return;
        }

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

    private bool CanSellPotion(string id)
    {
        if (levelComponent == null || levelComponent.CurrentStat == null)
            return true;

        int level = levelComponent.CurrentLevel;

        if (unlockPotionIDs == null || unlockPotionIDs.Count == 0)
            return true;

        int maxIndex = Mathf.Min(level, unlockPotionIDs.Count);

        for (int i = 0; i < maxIndex; i++)
        {
            if (unlockPotionIDs[i] == id)
                return true;
        }

        return false;
    }
}
