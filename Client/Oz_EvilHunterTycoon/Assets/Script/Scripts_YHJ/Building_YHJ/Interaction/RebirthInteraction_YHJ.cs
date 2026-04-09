using UnityEngine;

// 환생 기능
public class RebirthInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    void OnEnable()
    {
        EventBus_YHJ.RequestProcessUnit += OnProcessUnit;
    }

    void OnDisable()
    {
        EventBus_YHJ.RequestProcessUnit -= OnProcessUnit;
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("[Rebirth] 환생 요청");

        if (!unit.IsDead)
        {
            EventBus_YHJ.OnInteractionResult?.Invoke
            (
                unit,
                InteractionResult_YHJ.Fail
            );
            return;
        }

        queue.Enqueue(unit);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }

    private void OnProcessUnit(IUnit_YHJ unit, GameObject building)
    {
        if (building != gameObject)
            return;

        Debug.Log("[Rebirth] 큐 처리");

        if (!unit.IsDead)
        {
            Debug.Log("[Rebirth] 이미 상태 변경됨 → 취소");
            return;
        }

        // TODO: 실제 환생 처리
        // 예: unit.Rebirth();
        Debug.Log("[Rebirth] 실제 환생 처리 필요");

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
}