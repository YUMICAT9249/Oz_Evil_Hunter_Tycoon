using UnityEngine;

// ★ 상점 기능
public class ShopInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public string itemID = "Potion";   // 판매 아이템
    public int price = 10;             // 가격 (지금은 안 써도 됨)

    private BuildingInventory_YHJ inventory;
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        // 일단 무조건 가능 (나중에 돈 체크 가능)
        return true;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("상점 요청 들어옴");

        // ★ 재고 있으면 바로 지급
        if (inventory.TryConsume(itemID, 1))
        {
            Debug.Log("아이템 지급 성공");

            // TODO: 나중에 unit에 아이템 추가
            // unit.AddItem(itemID);

        }
        else
        {
            Debug.Log("재고 없음 → 대기열");

            queue.Enqueue(unit);
        }
    }
}