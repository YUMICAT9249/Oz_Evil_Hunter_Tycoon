using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingLevelComponent_YHJ : MonoBehaviour
{
    public BuildingInstance_YHJ instance;

    public Action<int> OnLevelChanged;
    public Action<LevelStat> OnLevelStatChanged;
    public event Action OnUpgraded;

    public int CurrentLevel => IsValid() ? instance.currentLevel : 0;
    public int MaxLevel => IsValid() ? instance.levelData.MaxLevel : 0;
    public LevelStat CurrentStat => IsValid() ? instance.CurrentStat : null;

    void Start()
    {
        if (!IsValid())
            return;

        OnLevelChanged?.Invoke(CurrentLevel);
        OnLevelStatChanged?.Invoke(CurrentStat);
    }

    public bool CanUpgrade()
    {
        if (instance == null || instance.levelData == null)
            return false;

        if (CurrentLevel >= MaxLevel)
            return false;

        LevelStat nextStat = instance.levelData.levelStats[CurrentLevel];
        return Manager_KJG.Currency.Gold >= nextStat.upgradeCost;
    }

    public bool TryUpgrade()
    {
        if (instance == null)
            return false;

        bool result = instance.TryUpgrade();

        if (result)
        {
            // UI에서 TryUpgrade()만 호출해도 업그레이드 성공 사운드가 같이 나도록 여기서 처리합니다.
            Manager_KJG.Audio?.PlaySFX("AFG1350");
            OnLevelChanged?.Invoke(CurrentLevel);
            OnLevelStatChanged?.Invoke(CurrentStat);
            OnUpgraded?.Invoke();

            if (Manager_KJG.SaveLoad != null)
                Manager_KJG.SaveLoad.GameSave();
        }

        return result;
    }

    public bool CanUseItem(string itemID)
    {
        if (!IsValid() || CurrentStat == null)
            return true;

        List<ItemData_YHJ> unlockedItems = GetUnlockedItems();
        bool hasItemReference = unlockedItems != null && unlockedItems.Count > 0;
        bool hasItemID = CurrentStat.unlockItemIDs != null && CurrentStat.unlockItemIDs.Count > 0;

        if (!hasItemReference && !hasItemID)
            return true;

        if (hasItemReference)
        {
            foreach (ItemData_YHJ itemData in unlockedItems)
            {
                if (itemData != null && itemData.IsSameItem(itemID))
                    return true;
            }
        }

        return hasItemID && CurrentStat.unlockItemIDs.Contains(itemID);
    }

    public List<ItemData_YHJ> GetUnlockedItems()
    {
        if (!IsValid() || CurrentStat == null || CurrentStat.unlockItems == null)
            return null;

        return CurrentStat.unlockItems;
    }

    public void RefreshLevelState()
    {
        if (!IsValid())
            return;

        OnLevelChanged?.Invoke(CurrentLevel);
        OnLevelStatChanged?.Invoke(CurrentStat);
    }

    private bool IsValid()
    {
        return instance != null && instance.levelData != null;
    }
}
