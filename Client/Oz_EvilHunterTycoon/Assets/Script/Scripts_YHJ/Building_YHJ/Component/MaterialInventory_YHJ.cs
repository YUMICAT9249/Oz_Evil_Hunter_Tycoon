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

    /// <summary>
    /// Save/Load를 위해 모든 재료 데이터를 반환합니다.
    /// </summary>
    public List<SaveLoadManager_KJG.MaterialSaveData> GetAllMaterials()
    {
        List<SaveLoadManager_KJG.MaterialSaveData> list = new List<SaveLoadManager_KJG.MaterialSaveData>();
        foreach (var item in items)
        {
            list.Add(new SaveLoadManager_KJG.MaterialSaveData
            {
                itemType = (DropItemType)System.Enum.Parse(typeof(DropItemType), item.Key),
                amount = item.Value
            });
        }
        return list;
    }

    /// <summary>
    /// SaveData에서 재료를 로드합니다.
    /// </summary>
    public void LoadMaterials(List<SaveLoadManager_KJG.MaterialSaveData> materials)
    {
        items.Clear();
        foreach (var m in materials)
        {
            string key = m.itemType.ToString();
            items[key] = m.amount;
        }
        Debug.Log("[BuildingInventory_YHJ] 재료 로드 완료");
    }
}