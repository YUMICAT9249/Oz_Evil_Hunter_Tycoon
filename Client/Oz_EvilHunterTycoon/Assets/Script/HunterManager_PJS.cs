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

        Debug.Log($"[HunterManager_PJS] Start - levelComponent: {(levelComponent != null ? levelComponent.name : "null")}");
        Debug.Log("[HunterManager_PJS] Initial spawn request");

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
        Debug.Log("[HunterManager_PJS] OnRequestSpawnHunter received");
        SpawnHunterToWaiting();
        OnWaitingListChanged?.Invoke();
    }

    public List<HunterController_PJS> GetWaitingHunters()
    {
        return new List<HunterController_PJS>(_waitingHunters);
    }

    public int GetWaitingCount()
    {
        return _waitingHunters.Count;
    }

    public int GetActiveCount()
    {
        return _activeHunters.Count;
    }

    public int GetWaitingCapacity()
    {
        return maxWaitingHunters;
    }

    public int GetVillageCapacity()
    {
        return maxVillageHunters;
    }

    public int GetTotalCount()
    {
        return _activeHunters.Count + _waitingHunters.Count;
    }

    public int GetTotalCapacity()
    {
        return maxVillageHunters + maxWaitingHunters;
    }

    public bool CanSpawnWaitingHunter()
    {
        if (_waitingHunters.Count >= maxWaitingHunters) return false;
        if (GetTotalCount() >= GetTotalCapacity()) return false;
        return true;
    }

    public void AddExpToHuntersInArea(int expAmount, AreaType areaType)
    {
        foreach (var hunter in _activeHunters)
        {
            if (hunter == null) continue;

            var data = hunter.GetComponent<HunterData_PJS>();
            if (data != null && data._areaType == areaType)
            {
                data.AddExp(expAmount);
            }
        }
    }


    // 스폰

    public HunterController_PJS SpawnHunterToWaiting()
    {
        Debug.Log("[HunterManager_PJS] SpawnHunterToWaiting called");

        if (!CanSpawnWaitingHunter())
        {
            Debug.Log($"[HunterManager_PJS] Spawn canceled - waiting/full active:{_activeHunters.Count} waiting:{_waitingHunters.Count}");
            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogWarning("[HunterManager_PJS] Spawn canceled - spawnPoint is null");
            return null;
        }

        HunterJop jop = SelectBalancedJob();
        Debug.Log($"[HunterManager_PJS] Balanced job selected: {jop}");
        GameObject[] prefabs = JopSelect(jop);
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning($"[HunterManager_PJS] Spawn canceled - no prefabs for job {jop}");
            return null;
        }

        GameObject selectedPrefab = SelectPrefabVariant(jop, prefabs);
        if (selectedPrefab == null)
        {
            Debug.LogWarning($"[HunterManager_PJS] Spawn canceled - no selectable prefab for job {jop}");
            return null;
        }

        GameObject obj = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"[HunterManager_PJS] Spawned object: {obj.name} at {spawnPoint.position}");

        var data = obj.GetComponent<HunterData_PJS>();
        if (data != null) data.SettingHunterData(jop);
        else Debug.LogWarning("[HunterManager_PJS] Spawned object has no HunterData_PJS");

        var controller = obj.GetComponent<HunterController_PJS>();
        if (controller == null)
        {
            Debug.LogWarning("[HunterManager_PJS] Spawned object has no HunterController_PJS");
            return null;
        }

        if (_waitingHunters.Count < maxWaitingHunters)
        {
            _waitingHunters.Add(controller);
            Debug.Log($"[HunterManager_PJS] Added to waiting list - waiting: {_waitingHunters.Count}/{maxWaitingHunters}");
        }
        else
        {
            _waitingQueue.Enqueue(controller);
            Debug.Log($"[HunterManager_PJS] Waiting full - queued hunter. Queue count: {_waitingQueue.Count}");
        }

        return controller;
    }

    private HunterJop SelectBalancedJob()
    {
        List<HunterJop> availableJobs = new List<HunterJop>();
        int minCount = int.MaxValue;

        AddCandidateJob(HunterJop.Berserker, berserkerPrefabs, availableJobs, ref minCount);
        AddCandidateJob(HunterJop.Paladin, paladinPrefabs, availableJobs, ref minCount);
        AddCandidateJob(HunterJop.Ranger, rangerPrefabs, availableJobs, ref minCount);
        AddCandidateJob(HunterJop.Sorcerer, sorcererPrefabs, availableJobs, ref minCount);

        if (availableJobs.Count == 0)
        {
            return HunterJop.Berserker;
        }

        return availableJobs[Random.Range(0, availableJobs.Count)];
    }

    private void AddCandidateJob(HunterJop job, GameObject[] prefabs, List<HunterJop> candidates, ref int minCount)
    {
        if (prefabs == null || prefabs.Length == 0) return;

        int currentCount = GetHunterCountByJob(job);

        if (currentCount < minCount)
        {
            minCount = currentCount;
            candidates.Clear();
            candidates.Add(job);
            return;
        }

        if (currentCount == minCount)
        {
            candidates.Add(job);
        }
    }

    private int GetHunterCountByJob(HunterJop job)
    {
        int count = 0;

        count += CountHuntersByJob(_activeHunters, job);
        count += CountHuntersByJob(_waitingHunters, job);

        return count;
    }

    private int CountHuntersByJob(List<HunterController_PJS> hunters, HunterJop job)
    {
        int count = 0;

        foreach (var hunter in hunters)
        {
            if (hunter == null) continue;

            HunterData_PJS data = hunter.GetComponent<HunterData_PJS>();
            if (data != null && data._hunterJop == job)
            {
                count++;
            }
        }

        return count;
    }

    private GameObject SelectPrefabVariant(HunterJop job, GameObject[] prefabs)
    {
        List<GameObject> availablePrefabs = new List<GameObject>();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;

            string prefabName = prefab.name;
            if (!HasHunterWithPrefabName(job, prefabName))
            {
                availablePrefabs.Add(prefab);
            }
        }

        if (availablePrefabs.Count > 0)
        {
            GameObject selected = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            Debug.Log($"[HunterManager_PJS] Selected unique variant: {selected.name}");
            return selected;
        }

        GameObject fallback = prefabs[Random.Range(0, prefabs.Length)];
        Debug.Log($"[HunterManager_PJS] All variants already used for {job}. Fallback variant: {fallback.name}");
        return fallback;
    }

    private bool HasHunterWithPrefabName(HunterJop job, string prefabName)
    {
        return HasHunterWithPrefabName(_activeHunters, job, prefabName) ||
               HasHunterWithPrefabName(_waitingHunters, job, prefabName);
    }

    private bool HasHunterWithPrefabName(List<HunterController_PJS> hunters, HunterJop job, string prefabName)
    {
        foreach (var hunter in hunters)
        {
            if (hunter == null) continue;

            HunterData_PJS data = hunter.GetComponent<HunterData_PJS>();
            if (data == null || data._hunterJop != job) continue;

            string hunterName = hunter.gameObject.name.Replace("(Clone)", "").Trim();
            if (hunterName == prefabName)
            {
                return true;
            }
        }

        return false;
    }

    private GameObject[] JopSelect(HunterJop jop)
    {
        if (jop == HunterJop.Berserker) return berserkerPrefabs;
        if (jop == HunterJop.Paladin) return paladinPrefabs;
        if (jop == HunterJop.Ranger) return rangerPrefabs;
        if (jop == HunterJop.Sorcerer) return sorcererPrefabs;
        return null;
    }

    // 대기 → 마을 이동
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

    // 추방
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
        Debug.Log($"[HunterManager_PJS] SendPopulation - active: {_activeHunters.Count}, waiting: {_waitingHunters.Count}, total: {GetTotalCount()}/{GetTotalCapacity()}");
        EventBus_YHJ.OnPopulationResult?.Invoke(GetTotalCount(), GetTotalCapacity());
    }
}
