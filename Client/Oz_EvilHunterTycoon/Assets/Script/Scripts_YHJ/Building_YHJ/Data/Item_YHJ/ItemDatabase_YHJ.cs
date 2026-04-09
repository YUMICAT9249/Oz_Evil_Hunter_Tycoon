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
        Instance = this;

        foreach (var item in items)
        {
            if (item == null) continue;

            map[item.itemID] = item;
        }
    }

    public ItemData_YHJ Get(string id)
    {
        if (map.TryGetValue(id, out var data))
            return data;

        return null;
    }
}