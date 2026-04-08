using UnityEngine;

public class BuildingProcessor_YHJ : MonoBehaviour
{
    public float processInterval = 2f;
    private float timer;

    private BuildingQueue_YHJ queue;

    void Awake()
    {
        queue = GetComponent<BuildingQueue_YHJ>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < processInterval)
            return;

        timer = 0f;

        if (queue == null)
            return;

        if (!queue.TryDequeue(out var unit))
            return;

        Debug.Log($"[Processor] 처리 시작: {unit}");
        EventBus_YHJ.RequestProcessUnit?.Invoke(unit, gameObject);
    }
}