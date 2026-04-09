using UnityEngine;

// ★ 수련장 기능
public class TrainInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    private BuildingQueue_YHJ queue;
    private BuildingLevelComponent_YHJ levelComponent;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
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

        if (unit == null || unit.IsDead)
            return;

        int trainingGoldCost = 0;
        float trainingDuration = 1800f;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            trainingGoldCost = levelComponent.CurrentStat.trainingGoldCost;
            trainingDuration = levelComponent.CurrentStat.trainingDuration;
        }

        // ★ 골드 차감 처리 (헌터팀 / 경제팀 연결)
        // ★ 수련서 사용 처리 (인벤/UI팀 연결)
        // ★ 30분 고정 상태 처리 (헌터 상태 시스템 연결)
        // ★ 환생 횟수별 필드 접근 제한 (헌터팀 연결)

        Debug.Log($"[Train] cost={trainingGoldCost}, duration={trainingDuration}");

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
}