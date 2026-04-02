using UnityEngine;

// ★ 기능: 훈련소 (훈련 요청 처리)
public class TrainInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        // ★ 기능: 살아있는 유닛만 훈련 가능
        return !unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("훈련 요청");

        // ★ 기능: 훈련 대기열 등록
        queue.Enqueue(unit);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }
}