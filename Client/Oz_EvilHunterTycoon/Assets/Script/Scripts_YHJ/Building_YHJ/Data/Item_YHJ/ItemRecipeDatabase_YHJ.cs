using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemRecipeDatabase_YHJ : MonoBehaviour
{
    public static ItemRecipeDatabase_YHJ Instance;

    public List<ItemRecipe_YHJ> recipes = new List<ItemRecipe_YHJ>();

    private Dictionary<string, ItemRecipe_YHJ> map =
        new Dictionary<string, ItemRecipe_YHJ>();

    void Awake()
    {
#if UNITY_EDITOR
        RefreshEditorRecipeList();
#endif

        if (Instance != null && Instance != this)
        {
            Debug.LogError("[ItemRecipeDatabase] duplicate instance");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        map.Clear();

        foreach (ItemRecipe_YHJ recipe in recipes)
        {
            if (recipe == null || recipe.resultItem == null)
                continue;

            string itemID = recipe.ResultItemID;
            if (string.IsNullOrEmpty(itemID))
                continue;

            if (map.ContainsKey(itemID))
            {
                Debug.LogError($"[ItemRecipeDatabase] duplicate result item: {itemID}");
                continue;
            }

            map[itemID] = recipe;
        }
    }

#if UNITY_EDITOR
    private void RefreshEditorRecipeList()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemRecipe_YHJ", new[] { "Assets" });
        List<ItemRecipe_YHJ> loadedRecipes = new List<ItemRecipe_YHJ>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemRecipe_YHJ recipe = AssetDatabase.LoadAssetAtPath<ItemRecipe_YHJ>(path);

            if (recipe != null)
                loadedRecipes.Add(recipe);
        }

        recipes = loadedRecipes;
    }
#endif

    public ItemRecipe_YHJ GetByItemID(string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
            return null;

        if (map.TryGetValue(itemID, out ItemRecipe_YHJ recipe))
            return recipe;

        return null;
    }

    public ItemRecipe_YHJ GetByItem(ItemData_YHJ itemData)
    {
        if (itemData == null)
            return null;

        return GetByItemID(itemData.ItemKey);
    }

    public List<ItemRecipe_YHJ> GetUnlockedRecipes(BuildingLevelComponent_YHJ levelComponent)
    {
        List<ItemRecipe_YHJ> unlockedRecipes = new List<ItemRecipe_YHJ>();
        if (levelComponent == null)
            return unlockedRecipes;

        List<ItemData_YHJ> unlockedItems = levelComponent.GetUnlockedItems();
        if (unlockedItems != null && unlockedItems.Count > 0)
        {
            foreach (ItemData_YHJ itemData in unlockedItems)
            {
                ItemRecipe_YHJ recipe = GetByItem(itemData);
                if (recipe != null)
                    unlockedRecipes.Add(recipe);
            }

            return unlockedRecipes;
        }

        foreach (ItemRecipe_YHJ recipe in recipes)
        {
            if (recipe == null || recipe.resultItem == null)
                continue;

            if (levelComponent.CanUseItem(recipe.ResultItemID))
                unlockedRecipes.Add(recipe);
        }

        return unlockedRecipes;
    }
}
