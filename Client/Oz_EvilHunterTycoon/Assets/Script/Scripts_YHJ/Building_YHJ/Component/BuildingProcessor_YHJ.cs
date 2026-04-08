using UnityEngine;

public class BuildingProcessor_YHJ : MonoBehaviour
{
    public float processInterval = 2f;
    private float timer;

    private BuildingQueue_YHJ queue;
    private BuildingLevelComponent_YHJ levelComponent;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < GetCurrentInterval())
            return;

        timer = 0f;

        if (queue == null)
            return;

        if (!queue.TryDequeue(out var unit))
            return;

        Debug.Log($"[Processor] 처리 시작: {unit}");
        EventBus_YHJ.RequestProcessUnit?.Invoke(unit, gameObject);
    }

    private float GetCurrentInterval()
    {
        if (levelComponent == null)
            return processInterval;

        if (levelComponent.CurrentStat == null)
            return processInterval;

        float speed = levelComponent.CurrentStat.workSpeed;

        if (speed <= 0f)
            return processInterval;

        return processInterval / speed;
    }
}