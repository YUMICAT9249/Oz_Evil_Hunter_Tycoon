using System.Collections.Generic;
using UnityEngine;

// ★ 건물 재고 시스템
public class BuildingInventory_YHJ : MonoBehaviour
{
    // ★ 아이템 데이터
    private Dictionary<string, int> items =
        new Dictionary<string, int>();

    // ★ 아이템 존재 여부
    public bool HasItem(string itemID)
    {
        return items.ContainsKey(itemID)
               && items[itemID] > 0;
    }

    // ★ 아이템 추가
    public void AddItem(string itemID, int amount)
    {
        if (!items.ContainsKey(itemID))
        {
            items[itemID] = 0;
        }

        items[itemID] += amount;

        Debug.Log($"[Inventory] {itemID} 추가됨: {items[itemID]}");
    }

    // ★ 아이템 소비
    public bool TryConsume(string itemID, int amount)
    {
        if (!HasItem(itemID))
        {
            Debug.Log($"[Inventory] {itemID} 없음");
            return false;
        }

        items[itemID] -= amount;

        Debug.Log($"[Inventory] {itemID} 소비됨: {items[itemID]}");

        return true;
    }

    // ★ 현재 수량 확인 (디버그용)
    public int GetAmount(string itemID)
    {
        if (!items.ContainsKey(itemID))
            return 0;

        return items[itemID];
    }
}