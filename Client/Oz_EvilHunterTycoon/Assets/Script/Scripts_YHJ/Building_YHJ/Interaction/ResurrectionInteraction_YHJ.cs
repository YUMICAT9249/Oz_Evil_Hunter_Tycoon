using UnityEngine;
using System.Collections;

// ¡Ú ¼º¼Ò - ºÎÈ° ±â´É
public class ResurrectionInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    [SerializeField] private Transform revivePoint;

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

        StartCoroutine(ProcessRevive(unit));
    }

    private IEnumerator ProcessRevive(IUnit_YHJ unit)
    {
        Vector3 revivePosition = revivePoint != null
            ? revivePoint.position
            : transform.position;

        if (unit is Component unitComponent)
        {
            unitComponent.transform.position = revivePosition;
        }

        int reviveGoldCost = 30;
        float reviveDelay = 30f;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            reviveGoldCost = levelComponent.CurrentStat.reviveGoldCost;
            reviveDelay = levelComponent.CurrentStat.reviveDelay;
        }

        // ¡Ú ÇåÅÍ °ñµå Â÷°¨ Ã³¸® (ÇåÅÍÆÀ / °æÁ¦ÆÀ ¿¬°á)
        // if (!unit.TrySpendGold(reviveGoldCost)) yield break;

        yield return new WaitForSeconds(reviveDelay);

        unit.Revive();

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }
}