using UnityEngine;

// ★ 마을회관 기반 헌터 생성 요청 시스템
public class TownHallSpawnController_YHJ : MonoBehaviour
{
    public float spawnInterval = 5f;
    private float timer;

    private bool canSpawn = false;

    void OnEnable()
    {
        EventBus_YHJ.OnPopulationResult += OnPopulationResult;
    }

    void OnDisable()
    {
        EventBus_YHJ.OnPopulationResult -= OnPopulationResult;
    }

    void Update()
    {
        // ★ 인구 상태 요청
        EventBus_YHJ.RequestPopulation?.Invoke();

        if (!canSpawn) return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            Debug.Log("헌터 생성 요청");

            EventBus_YHJ.RequestSpawnHunter?.Invoke();
        }
    }

    void OnPopulationResult(int current, int max)
    {
        canSpawn = current < max;
    }
}