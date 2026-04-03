using UnityEngine;

// 환생 기능
public class RebirthInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("환생 요청");

        if (!unit.IsDead)
        {
            Debug.Log("죽은 상태 아님");
            return;
        }

        // 큐 등록
        queue.Enqueue(unit);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }
}