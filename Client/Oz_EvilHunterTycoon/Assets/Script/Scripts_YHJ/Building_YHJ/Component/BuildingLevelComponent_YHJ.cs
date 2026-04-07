using UnityEngine;

public class BuildingLevelComponent_YHJ : MonoBehaviour
{
    public BuildingInstance_YHJ instance;

    public int CurrentLevel => instance.currentLevel;
    public int MaxLevel => instance.levelData.MaxLevel;

    public LevelStat CurrentStat => instance.CurrentStat;

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
}