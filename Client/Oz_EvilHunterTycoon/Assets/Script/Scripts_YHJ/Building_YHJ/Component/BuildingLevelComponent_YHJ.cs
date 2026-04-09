using UnityEngine;

public class BuildingLevelComponent_YHJ : MonoBehaviour
{
    public BuildingInstance_YHJ instance;

    // ★ 레벨 변경 이벤트
    public System.Action<int> OnLevelChanged;

    // ★ 현재 레벨 데이터 변경 이벤트 (UI / 협업용)
    public System.Action<LevelStat> OnLevelStatChanged;

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

    public bool CanUpgrade(int gold)
    {
        if (instance == null || instance.levelData == null)
            return false;

        if (CurrentLevel >= MaxLevel)
            return false;

        var nextStat = instance.levelData.levelStats[CurrentLevel];

        return gold >= nextStat.upgradeCost;
    }

    public bool TryUpgrade(ref int gold)
    {
        if (instance == null)
            return false;

        bool result = instance.TryUpgrade(ref gold);

        if (result)
        {
            OnLevelChanged?.Invoke(CurrentLevel);
            OnLevelStatChanged?.Invoke(CurrentStat);
        }

        return result;
    }

    // ★ 현재 레벨에서 이 아이템 사용/제작 가능한지
    public bool CanUseItem(string itemID)
    {
        if (!IsValid() || CurrentStat == null)
            return true;

        if (CurrentStat.unlockItemIDs == null || CurrentStat.unlockItemIDs.Count == 0)
            return true;

        return CurrentStat.unlockItemIDs.Contains(itemID);
    }

    // ★ 현재 레벨 데이터 강제 갱신 알림
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