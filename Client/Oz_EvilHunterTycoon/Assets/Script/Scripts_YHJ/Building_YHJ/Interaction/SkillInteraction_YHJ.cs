using UnityEngine;

// ★ 스킬 기능 (Queue 기반으로 변경)
public class SkillInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return !unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("[Skill] 대기열 등록");

        if (unit.IsDead)
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
}