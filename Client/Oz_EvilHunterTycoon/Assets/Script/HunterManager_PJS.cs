using UnityEngine;
using System.Collections.Generic;

public class HunterManager_PJS : BaseManager_KJG<HunterManager_PJS>
{
    [Header("구역 설정")]
    [SerializeField] private BoxCollider2D[] _allArea;

    [Header("직업 프리팹")]
    public GameObject[] berserkerPrefabs;
    public GameObject[] paladinPrefabs;
    public GameObject[] rangerPrefabs;
    public GameObject[] sorcererPrefabs;

    [Header("스폰")]
    public Transform spawnPoint;

    [Header("리스트")]
    public List<HunterController_PJS> _activeHunters = new List<HunterController_PJS>();
    public List<HunterController_PJS> _waitingHunters = new List<HunterController_PJS>();

    [Header("수용량")]
    [SerializeField] private int maxWaitingHunters = 3;
    [SerializeField] private int maxVillageHunters = 4;

    public BuildingLevelComponent_YHJ levelComponent;

    public System.Action OnWaitingListChanged;

    private Queue<HunterController_PJS> _waitingQueue = new Queue<HunterController_PJS>();

    float moveTimer;
    float moveInterval = 3f;

    protected override void Start()
    {
        levelComponent = FindObjectOfType<BuildingLevelComponent_YHJ>();
        base.Start();

        EventBus_YHJ.RequestSpawnHunter?.Invoke(); // 시작 1명
    }

    void OnEnable()
    {
        EventBus_YHJ.RequestSpawnHunter += OnRequestSpawnHunter;
        EventBus_YHJ.RequestPopulation += SendPopulation;
    }

    void OnDisable()
    {
        EventBus_YHJ.RequestSpawnHunter -= OnRequestSpawnHunter;
        EventBus_YHJ.RequestPopulation -= SendPopulation;
    }

    void Update()
    {
        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            maxWaitingHunters = levelComponent.CurrentStat.waitingCapacity;
            maxVillageHunters = levelComponent.CurrentStat.capacity;
        }

        moveTimer += Time.deltaTime;

        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            TryMoveWaitingHunterToVillage();
        }
    }

    private void OnRequestSpawnHunter()
    {
        SpawnHunterToWaiting();
        OnWaitingListChanged?.Invoke();
    }

    public List<HunterController_PJS> GetWaitingHunters()
    {
        return new List<HunterController_PJS>(_waitingHunters);
    }

    // =========================
    // 스폰
    // =========================
    public HunterController_PJS SpawnHunterToWaiting()
    {
        if (spawnPoint == null) return null;

        HunterJop jop = (HunterJop)Random.Range(1, 5);
        GameObject[] prefabs = JopSelect(jop);
        if (prefabs == null || prefabs.Length == 0) return null;

        GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)], spawnPoint.position, Quaternion.identity);

        var data = obj.GetComponent<HunterData_PJS>();
        if (data != null) data.SettingHunterData(jop);

        var controller = obj.GetComponent<HunterController_PJS>();
        if (controller == null) return null;

        if (_waitingHunters.Count < maxWaitingHunters)
        {
            _waitingHunters.Add(controller);
        }
        else
        {
            _waitingQueue.Enqueue(controller);
        }

        return controller;
    }

    private GameObject[] JopSelect(HunterJop jop)
    {
        if (jop == HunterJop.Berserker) return berserkerPrefabs;
        if (jop == HunterJop.Paladin) return paladinPrefabs;
        if (jop == HunterJop.Ranger) return rangerPrefabs;
        if (jop == HunterJop.Sorcerer) return sorcererPrefabs;
        return null;
    }

    // =========================
    // 대기 → 마을 이동
    // =========================
    private void TryMoveWaitingHunterToVillage()
    {
        if (_waitingHunters.Count == 0) return;
        if (_activeHunters.Count >= maxVillageHunters) return;

        var hunter = _waitingHunters[0];
        _waitingHunters.RemoveAt(0);

        _activeHunters.Add(hunter);

        if (_waitingQueue.Count > 0)
        {
            _waitingHunters.Add(_waitingQueue.Dequeue());
        }

        OnWaitingListChanged?.Invoke();
    }

    // =========================
    // 추방
    // =========================
    public void RemoveWaitingHunter(HunterController_PJS hunter)
    {
        if (_waitingHunters.Remove(hunter))
        {
            _waitingQueue.Enqueue(hunter);
            OnWaitingListChanged?.Invoke();
        }
    }

    void SendPopulation()
    {
        EventBus_YHJ.OnPopulationResult?.Invoke(_activeHunters.Count, 999);
    }
}