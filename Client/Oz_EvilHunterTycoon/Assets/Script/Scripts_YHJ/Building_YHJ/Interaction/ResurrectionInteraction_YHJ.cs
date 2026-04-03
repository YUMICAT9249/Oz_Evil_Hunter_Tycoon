using UnityEngine;

// ★ 부활 기능
public class ResurrectionInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
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
        Debug.Log("부활 요청");

        if (!unit.IsDead)
        {
            Debug.Log("이미 살아있음");
            return;
        }

        // 일단 큐에 넣는다
        queue.Enqueue(unit);

        // 결과 이벤트 (선택)
        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }
}