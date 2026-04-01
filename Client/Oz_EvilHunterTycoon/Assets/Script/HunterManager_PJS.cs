using UnityEngine;
using System.Collections.Generic;

// 헌터 제어 스크립트

public enum AreaType
{
    Village = 0,
    AreaA = 1,
    AreaB = 2,
    AreaC = 3,
    AreaFieldBoss = 4,
    AreaWorldBoss = 5,
    AreaDevilCastle = 6
}

public class HunterManager_PJS : MonoBehaviour
{
    [Header("구역 설정(AreaType 순서대로 배치)")]
    [SerializeField] private BoxCollider2D[] _allArea;

    [Header("직업별 외형 프리팹 (A,B,C)")]
    public GameObject[] berserkerPrefabs;
    public GameObject[] paladinPrefabs;
    public GameObject[] rangerPrefabs;
    public GameObject[] sorcererPrefabs;

    [Header("스폰 설정")]
    public Transform spawnPoint;

    [Header("활성화된 헌터 리스트")]
    public List<HunterController_PJS> _activeHunters = new List<HunterController_PJS>();

    [Header("구역(공통 변수)")]
    public AreaType _areaType; // 호출할 구역 타입

    void Start()
    {
        if (EventManager_KJG.Instance != null)
        {
            // 헌터 방문 구독
            EventManager_KJG.Instance.AddListener(EventManager_KJG.GameEvent.RefreshUI, HunterRandomSpawn);
            // 보스 출현시 강제이동 구독
            EventManager_KJG.Instance.AddListener(EventManager_KJG.GameEvent.BossDefeated, CallAllHuntersToArea);
        }

        HunterController_PJS[] findingHunters = FindObjectsOfType<HunterController_PJS>();
        BoxCollider2D villageBox = GetAreaCollider(AreaType.Village);

        for (int i = 0; i < findingHunters.Length; i++)
        { 
            _activeHunters.Add(findingHunters[i]);
            findingHunters[i].SetArea(villageBox);
        }
    }

    // 헌터 직업 랜덤 생성
    public void HunterRandomSpawn()
    {
        HunterJop jop = (HunterJop)Random.Range(1, 5);
        GameObject[] jopSelect = GetJopSelect(jop);

        if (jopSelect == null || jopSelect.Length == 0) return;

        GameObject newHunter = Instantiate
            (
                jopSelect[Random.Range(0, jopSelect.Length)], 
                spawnPoint.position, 
                Quaternion.identity
            );
        HunterData_PJS hunterData = newHunter.GetComponent<HunterData_PJS>();
        if (hunterData != null)
        {
            hunterData.SettingHunterData(jop);
        }

        HunterController_PJS hunterController = newHunter.GetComponent<HunterController_PJS>();
        if (hunterController != null)
        {
            _activeHunters.Add(hunterController);
        }
    }

    private GameObject[] GetJopSelect(HunterJop jop)
    {
        if (jop == HunterJop.Berserker) return berserkerPrefabs;
        if (jop == HunterJop.Paladin) return paladinPrefabs;
        if (jop == HunterJop.Ranger) return rangerPrefabs;
        if (jop == HunterJop.Sorcerer) return sorcererPrefabs;
        return null;
    }

    // 보스 / 마왕성 보스 소환시 모든 헌터 강제 이동
    public void CallAllHuntersToArea()
    {
        for (int i = 0; i < _activeHunters.Count; i++)
        {
            HunterController_PJS hunterController = _activeHunters[i];
            if (hunterController != null)
            { 
                HunterData_PJS hunterData = hunterController.GetComponent<HunterData_PJS>();
                if (hunterData != null)
                {
                    // 보스 구역으로 변경
                    hunterData._areaType = AreaType.AreaFieldBoss;
                    // 실제 이동할 콜라이더 찾아서 SetArea에 넣음
                    BoxCollider2D bossArea = GetAreaCollider(AreaType.AreaFieldBoss);
                    hunterController.SetArea(bossArea);
                }
            }
        }
    }

    // 구역 전환 / _areaIndex 변수에 담긴 번호 -> 콜라이더를 반환
    public BoxCollider2D GetAreaCollider(AreaType type)
    {
        int index = (int)type;
        // 입력된 인덱스가 배열 범위 안에 있는지 확인
        if (index >= 0 && index < _allArea.Length)
        {
            return _allArea[index];
        }
        return null;
    }
}
