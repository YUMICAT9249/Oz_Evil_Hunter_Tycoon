using UnityEngine;

// ★ 부활 기능
public class ResurrectionInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    [SerializeField] private Transform revivePoint;
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
        Debug.Log("[Resurrection] 부활 요청");

        if (!unit.IsDead)
        {
            Debug.Log("[Resurrection] 이미 살아있음");
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

        Debug.Log("[Resurrection] 큐 처리");

        if (!unit.IsDead)
        {
            Debug.Log("[Resurrection] 이미 살아나서 처리 취소");
            return;
        }

        if (revivePoint == null)
        {
            Debug.LogWarning("[Resurrection] revivePoint 없음 → transform.position 사용");
        }

        Vector3 revivePosition = revivePoint != null
    ? revivePoint.position
    : transform.position;

        unit.Revive();

        if (unit is Component unitComponent)
        {
            if (unitComponent.TryGetComponent(out HunterController_PJS hunterController))
            {
                hunterController.transform.position = revivePosition;
            }
            else
            {
                unitComponent.transform.position = revivePosition;
            }
        }

        Debug.Log("[Resurrection] 부활 완료");

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
}