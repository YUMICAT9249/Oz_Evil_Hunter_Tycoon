using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemDatabase_YHJ : MonoBehaviour
{
    public static ItemDatabase_YHJ Instance;

    public List<ItemData_YHJ> items;

    private Dictionary<string, ItemData_YHJ> map =
        new Dictionary<string, ItemData_YHJ>();

    void Awake()
    {
#if UNITY_EDITOR
        // ★ YHJ: Assets/ItemData 아래의 ItemData_YHJ를 자동 수집해 새 장비 데이터가 누락되지 않도록 보조
        RefreshEditorItemList();
#endif

        if (Instance != null && Instance != this)
        {
            Debug.LogError("[ItemDatabase] duplicate instance");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        map.Clear();

        foreach (var item in items)
        {
            if (item == null)
                continue;

            if (map.ContainsKey(item.itemID))
            {
                Debug.LogError($"[ItemDatabase] duplicate ID: {item.itemID}");
                continue;
            }

            map[item.itemID] = item;
        }
    }

#if UNITY_EDITOR
    // ★ YHJ: 수동 리스트 관리 실수를 줄이기 위해 에디터에서 ItemData 에셋 목록을 자동으로 갱신
    private void RefreshEditorItemList()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemData_YHJ", new[] { "Assets/ItemData" });
        List<ItemData_YHJ> loadedItems = new List<ItemData_YHJ>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemData_YHJ item = AssetDatabase.LoadAssetAtPath<ItemData_YHJ>(path);

            if (item != null)
                loadedItems.Add(item);
        }

        items = loadedItems;
    }
#endif

    public ItemData_YHJ Get(string id)
    {
        if (map.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"[ItemDatabase] missing ID: {id}");
        return null;
    }

    // ★ YHJ TODO: HunterData_PJS.SetWeapon/SetArmor/SetGloves/SetBoots에서
    // itemID를 넘겨 장비 데이터 조회 후 슬롯/직업까지 한 번에 검사할 때 사용할 것
    public bool TryGetEquipmentData(string itemID, EquipmentSlot_YHJ slot, HunterJop hunterJop, out ItemData_YHJ itemData)
    {
        itemData = Get(itemID);

        if (itemData == null)
            return false;

        if (!itemData.IsMatchSlot(slot))
            return false;

        if (!itemData.CanEquip(hunterJop))
            return false;

        return true;
    }
}