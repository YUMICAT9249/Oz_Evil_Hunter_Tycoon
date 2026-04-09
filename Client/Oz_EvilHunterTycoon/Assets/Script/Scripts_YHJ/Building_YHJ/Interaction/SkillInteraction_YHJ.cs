using UnityEngine;

// ★ 성소 - 스킬 기능
public class SkillInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;
    private BuildingLevelComponent_YHJ levelComponent;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        return unit != null && !unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        if (unit == null || unit.IsDead)
        {
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        queue.Enqueue(unit);

        // ★ UI팀 / 헌터팀 연결
        // ★ 메인스킬 10레벨, 서브스킬 5레벨 구조는 여기서 UI 오픈 시 전달
        bool canUseMainSkill = false;
        bool canUseSubSkill = false;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            canUseMainSkill = levelComponent.CurrentStat.canUseMainSkill;
            canUseSubSkill = levelComponent.CurrentStat.canUseSubSkill;
        }

        // ★ UI 오픈 이벤트 연결
        // EventBus_YHJ.RequestOpenSkillUI?.Invoke(unit, canUseMainSkill, canUseSubSkill);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }
}