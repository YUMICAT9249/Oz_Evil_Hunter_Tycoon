using UnityEngine;

public class ArmorShopInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return true;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("[ArmorShop] 대기열 등록");

        queue.Enqueue(unit);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }
}