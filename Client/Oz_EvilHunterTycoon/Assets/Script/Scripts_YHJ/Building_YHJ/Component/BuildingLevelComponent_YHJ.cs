using UnityEngine;

public class BuildingLevelComponent_YHJ : MonoBehaviour
{
    public BuildingInstance_YHJ instance;

    public int CurrentLevel => IsValid() ? instance.currentLevel : 0;
    public int MaxLevel => IsValid() ? instance.levelData.MaxLevel : 0;

    public LevelStat CurrentStat => IsValid() ? instance.CurrentStat : null;

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

        return instance.TryUpgrade(ref gold);
    }
    private bool IsValid()
    {
        return instance != null && instance.levelData != null;
    }
}