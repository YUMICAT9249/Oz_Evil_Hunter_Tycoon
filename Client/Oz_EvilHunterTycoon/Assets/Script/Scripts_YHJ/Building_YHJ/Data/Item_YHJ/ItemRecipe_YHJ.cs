using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemRecipeCost_YHJ
{
    public ItemData_YHJ itemData;
    public int amount = 1;

    public bool IsValid()
    {
        return itemData != null && amount > 0;
    }
}

[System.Serializable]
public class ItemRecipeOption_YHJ
{
    public string optionName;
    public List<ItemRecipeCost_YHJ> costs = new List<ItemRecipeCost_YHJ>();

    public string DisplayName
    {
        get
        {
            return string.IsNullOrEmpty(optionName) ? "Default" : optionName;
        }
    }
}

[CreateAssetMenu(fileName = "ItemRecipe_YHJ", menuName = "YHJ/Item Recipe")]
public class ItemRecipe_YHJ : ScriptableObject
{
    public ItemData_YHJ resultItem;
    public int resultAmount = 1;
    public List<ItemRecipeOption_YHJ> options = new List<ItemRecipeOption_YHJ>();

    public string ResultItemID
    {
        get
        {
            return resultItem == null ? string.Empty : resultItem.ItemKey;
        }
    }

    public bool HasMultipleOptions => options != null && options.Count > 1;

    public bool IsMatchItem(string itemID)
    {
        return resultItem != null && resultItem.IsSameItem(itemID);
    }

    public ItemRecipeOption_YHJ GetOption(int optionIndex)
    {
        if (options == null || optionIndex < 0 || optionIndex >= options.Count)
            return null;

        return options[optionIndex];
    }

    public bool CanCraft(MaterialInventory_YHJ inventory, int craftCount, int optionIndex)
    {
        if (inventory == null || craftCount <= 0)
            return false;

        ItemRecipeOption_YHJ option = GetOption(optionIndex);
        if (option == null || option.costs == null || option.costs.Count == 0)
            return false;

        foreach (ItemRecipeCost_YHJ cost in option.costs)
        {
            if (cost == null || !cost.IsValid())
                return false;

            int requiredAmount = cost.amount * craftCount;
            if (!inventory.HasItem(cost.itemData.ItemKey, requiredAmount))
                return false;
        }

        return true;
    }

    public bool TryConsume(MaterialInventory_YHJ inventory, int craftCount, int optionIndex)
    {
        if (!CanCraft(inventory, craftCount, optionIndex))
            return false;

        ItemRecipeOption_YHJ option = GetOption(optionIndex);

        foreach (ItemRecipeCost_YHJ cost in option.costs)
        {
            int requiredAmount = cost.amount * craftCount;
            inventory.RemoveItem(cost.itemData.ItemKey, requiredAmount);
        }

        return true;
    }
}
