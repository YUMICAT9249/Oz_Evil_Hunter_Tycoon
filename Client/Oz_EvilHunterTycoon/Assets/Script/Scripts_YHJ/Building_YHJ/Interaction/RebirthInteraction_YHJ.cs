using UnityEngine;

// ★ 성소 - 환생 기능 (대기)
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
        return unit != null && unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        if (unit == null || !unit.IsDead)
        {
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
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

        if (unit == null || !unit.IsDead)
            return;

        // ★ 실제 환생 처리 (헌터팀 협업)
        // unit.Rebirth();
        // 환생 후 필드 접근 제한, 레벨 연동 등 연결 예정

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
}