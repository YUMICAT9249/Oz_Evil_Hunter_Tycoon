using System.Collections.Generic;
using UnityEngine;

// ★ 공용 소재 인벤토리
public class MaterialInventory_YHJ : MonoBehaviour
{
    public static MaterialInventory_YHJ Instance;

    private Dictionary<string, int> items = new Dictionary<string, int>();

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(string id, int amount)
    {
        if (!items.ContainsKey(id))
            items[id] = 0;

        items[id] += amount;

        Debug.Log($"[Material] {id} +{amount} → {items[id]}");

        // ★ UI팀이 소재 UI 붙일 때 여기 연결
        // EventBus_YHJ.OnMaterialChanged?.Invoke(id, items[id]);
    }

    public bool RemoveItem(string id, int amount)
    {
        if (!HasItem(id, amount))
            return false;

        items[id] -= amount;

        // ★ UI팀이 소재 UI 붙일 때 여기 연결
        // EventBus_YHJ.OnMaterialChanged?.Invoke(id, items[id]);

        return true;
    }

    public bool HasItem(string id, int amount)
    {
        return items.ContainsKey(id) && items[id] >= amount;
    }

    public int GetAmount(string id)
    {
        if (!items.ContainsKey(id))
            return 0;

        return items[id];
    }
}