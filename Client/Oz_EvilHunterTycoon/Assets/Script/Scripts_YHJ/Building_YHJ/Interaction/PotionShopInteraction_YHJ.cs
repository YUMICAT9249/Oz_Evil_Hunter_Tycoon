using UnityEngine;

// ★ 포션상점 (제작 + 판매)
public class PotionShopInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingInventory_YHJ inventory;
    private BuildingLevelComponent_YHJ levelComponent;

    public string potionID = "Potion";

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return true;
    }

    public void Interact(IUnit_YHJ unit)
    {
        if (!inventory.HasItem(potionID, 1))
        {
            TryMakePotion();
        }

        if (!inventory.HasItem(potionID, 1))
        {
            Debug.Log("[Potion] 재고 없음");
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        // ★ 헌터 골드 차감 / 포션 지급은 헌터팀, UI팀과 연결
        // inventory.RemoveItem(potionID, 1);
        // unit.AddItem(...);

        Manager_KJG.Audio?.PlaySFX("CD01042");
        EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Success);
    }

    private bool TryMakePotion()
    {
        if (levelComponent != null && !levelComponent.CanUseItem(potionID))
        {
            Debug.Log("[Potion] 현재 레벨에서 제작 불가");
            return false;
        }

        if (!MaterialInventory_YHJ.Instance.HasItem("Herb", 2))
            return false;

        MaterialInventory_YHJ.Instance.RemoveItem("Herb", 2);

        inventory.AddItem(potionID, 1);

        Debug.Log("[Potion] 제작 완료");

        // ★ UI 제작 완료 연결
        // EventBus_YHJ.OnCraftCompleted?.Invoke(potionID, 1);

        return true;
    }
}