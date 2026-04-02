using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public interface IBuildingInteraction_YHJ
{
    bool CanInteract(IUnit_YHJ unit);
    void Interact(IUnit_YHJ unit);
}