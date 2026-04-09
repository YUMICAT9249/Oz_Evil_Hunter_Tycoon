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

        // ★ 레벨 기반 최대 인구 계산
        maxPopulation = GetMaxPopulation();

        canSpawn = currentPopulation < maxPopulation;

        if (!canSpawn)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            Debug.Log($"헌터 생성 요청 ({currentPopulation}/{maxPopulation})");

            EventBus_YHJ.RequestSpawnHunter?.Invoke();
        }
    }

    void OnPopulationResult(int current, int max)
    {
        int levelMax = max;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            levelMax = levelComponent.CurrentStat.capacity;
        }

        canSpawn = current < levelMax;
    }

    private int GetMaxPopulation()
    {
        if (levelComponent == null || levelComponent.CurrentStat == null)
            return 0;

        return levelComponent.CurrentStat.capacity;
    }
}