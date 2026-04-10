using System.Collections.Generic;
using UnityEngine;

// ★ 건물 완제품 재고 시스템
public class BuildingInventory_YHJ : MonoBehaviour, IStringInventoryReader_YHJ, IStringInventoryWriter_YHJ
{
    public BuildingInstance_YHJ buildingInstance;
    private Dictionary<string, int> items =
        new Dictionary<string, int>();


    public Dictionary<string, int> GetAllItems()
    {
        return new Dictionary<string, int>(items);
    }

    // ★ 아이템 불러오기 (Load용)
    public void LoadItems(Dictionary<string, int> data)
    {
        items = new Dictionary<string, int>(data);
    }

    public bool HasItem(string itemID)
    {
        return items.ContainsKey(itemID)
               && items[itemID] > 0;
    }

    public void AddItem(string itemID, int amount)
    {
        if (!items.ContainsKey(itemID))
        {
            items[itemID] = 0;
        }

        items[itemID] += amount;

        Debug.Log($"[Inventory] {itemID} 추가됨: {items[itemID]}");

        // ★ UI팀이 건물 재고 UI 붙일 때 여기 연결
        // EventBus_YHJ.OnBuildingItemChanged?.Invoke(gameObject, itemID, items[itemID]);
    }

    public bool TryConsume(string itemID, int amount)
    {
        if (!HasItem(itemID))
        {
            Debug.Log($"[Inventory] {itemID} 없음");
            return false;
        }

        items[itemID] -= amount;

        Debug.Log($"[Inventory] {itemID} 소비됨: {items[itemID]}");

        // ★ UI팀이 건물 재고 UI 붙일 때 여기 연결
        // EventBus_YHJ.OnBuildingItemChanged?.Invoke(gameObject, itemID, items[itemID]);

        return true;
    }

    public int GetAmount(string itemID)
    {
        if (!items.ContainsKey(itemID))
            return 0;

        return items[itemID];
    }

    public int GetItemCount(string itemID)
    {
        return GetAmount(itemID);
    }

    public bool HasItem(string itemID, int amount)
    {
        return GetAmount(itemID) >= amount;
    }

    public bool RemoveItem(string itemID, int amount)
    {
        return TryConsume(itemID, amount);
    }
}