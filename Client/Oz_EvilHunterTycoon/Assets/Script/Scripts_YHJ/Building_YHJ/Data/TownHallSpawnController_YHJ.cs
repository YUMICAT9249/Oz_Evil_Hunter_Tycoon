using UnityEngine;

// ★ 마을회관 기반 헌터 생성 시스템 (레벨 연동)
public class TownHallSpawnController_YHJ : MonoBehaviour
{
    public float spawnInterval = 5f;
    private float timer;

    private bool canSpawn = false;

    private int currentPopulation = 0;
    private int maxPopulation = 0;

    private BuildingLevelComponent_YHJ levelComponent;

    void Awake()
    {
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

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
        // ★ 현재 인구 요청 (헌터팀)
        EventBus_YHJ.RequestPopulation?.Invoke();

        if (maxPopulation <= 0)
        {
            maxPopulation = GetFallbackMaxPopulation();
        }

        canSpawn = currentPopulation < maxPopulation;

        if (!canSpawn)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;

            EventBus_YHJ.RequestSpawnHunter?.Invoke();
        }
    }

    void OnPopulationResult(int current, int max)
    {
        currentPopulation = current;
        maxPopulation = max > 0 ? max : GetFallbackMaxPopulation();
        canSpawn = currentPopulation < maxPopulation;
    }

    private int GetFallbackMaxPopulation()
    {
        if (HunterManager_PJS.Instance != null)
        {
            return HunterManager_PJS.Instance.GetTotalCapacity();
        }

        if (levelComponent == null || levelComponent.CurrentStat == null)
            return 0;

        return levelComponent.CurrentStat.capacity;
    }
}
