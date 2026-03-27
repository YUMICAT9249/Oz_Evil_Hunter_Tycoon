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
    public static HunterManager_PJS Instance;

    [Header("구역 설정(AreaType 순서대로 배치)")]
    [SerializeField] private BoxCollider2D[] _allArea;

    [Header("활성화된 헌터 리스트")]
    public List<HunterController_PJS> _activeHunters = new List<HunterController_PJS>();

    [Header("구역(공통 변수)")]
    public AreaType _areaType; // 호출할 구역 타입

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // 보스 / 마왕성 보스 소환시 모든 헌터 강제 이동
    public void CallAllHuntersToArea()
    {
        for (int i = 0; i < _activeHunters.Count; i++)
        {
            if (_activeHunters[i] != null)
            {
                // 헌터 데이터의 위치 변경
                _activeHunters[i].GetComponent<HunterData_PJS>()._areaType = _areaType;
                // 헌터 스스로 위치 갱신
                _activeHunters[i].UpdateLocation();
            }
        }
    }

    // 구역 전환 / _areaIndex 변수에 담긴 번호 -> 콜라이더를 반환
    public BoxCollider2D GetAreaCollider(AreaType type)
    {
        int index = (int)type;
        // 1. 입력된 인덱스가 배열 범위 안에 있는지 확인
        if (index >= 0 && index < _allArea.Length)
        {
            return _allArea[index];
        }
        return null;
    }
}
