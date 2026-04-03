using UnityEngine;

// 치료소 기능
public class HealInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public string itemID = "Bandage";
    public float healAmount = 20f;

    private BuildingInventory_YHJ inventory;
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return unit.CurrentHP < unit.MaxHP;
    }

    public void Interact(IUnit_YHJ unit)
    {
        // 재고 있으면 바로 처리
        if (inventory.TryConsume(itemID, 1))
        {
            Debug.Log("치료 실행");
            unit.Heal(healAmount);
        }
        else
        {
            Debug.Log("재고 없음 → 대기열");

            queue.Enqueue(unit);
        }
    }
}