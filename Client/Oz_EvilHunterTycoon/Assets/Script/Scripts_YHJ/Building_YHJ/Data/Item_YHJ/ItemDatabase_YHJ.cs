using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase_YHJ : MonoBehaviour
{
    public static ItemDatabase_YHJ Instance;

    public List<ItemData_YHJ> items;

    private Dictionary<string, ItemData_YHJ> map =
        new Dictionary<string, ItemData_YHJ>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("[ItemDatabase] 중복 생성됨");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (var item in items)
        {
            if (item == null) continue;

            if (map.ContainsKey(item.itemID))
            {
                Debug.LogError($"[ItemDatabase] 중복 ID: {item.itemID}");
                continue;
            }

            map[item.itemID] = item;
        }
    }

    public ItemData_YHJ Get(string id)
    {
        if (map.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"[ItemDatabase] 없는 ID 요청: {id}");
        return null;
    }
}