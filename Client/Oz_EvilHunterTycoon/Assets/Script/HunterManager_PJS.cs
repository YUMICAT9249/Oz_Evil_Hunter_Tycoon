using System.Collections.Generic;
using UnityEngine;

public class HunterManager_PJS : MonoBehaviour
{
    public static HunterManager_PJS Instance;

    [Header("구역 설정(AreaType 순서대로 배치)")]
    [SerializeField] private BoxCollider2D[] _allArea;

    [Header("활성화된 헌터 리스트")]
    public List<HunterController_PJS> _activeHunters = new List<HunterController_PJS>();

    [Header("헌터(공통 변수)")]
    public HunterJop _randomJop;  // 헌터이름을 랜덤으로 생성할 직업타입
    public string _nameList;      // 랜덤으로 생성된 이름을 담는 변수

    [Header("구역(공통 변수)")]
    public AreaType _areaType; // 호출할 구역 타입
    public int _areaIndex;     // 찾을 구역 배열 번호

    // 직업별 헌터 이름
    private List<string> beserkerNames = new List<string> { "브란", "샤론", "세나" };
    private List<string> paladinNames = new List<string> { "카일", "알프", "홉스" };
    private List<string> rangerNames = new List<string> { "카이즈", "바레인", "크리샤" };
    private List<string> sorcererNames = new List<string> { "라글라스", "두아트린", "브리디도" };

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
        _areaIndex = (int)_areaType;
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
    public BoxCollider2D GetAreaCollider()
    {
        // 1. 입력된 인덱스가 배열 범위 안에 있는지 확인
        if (_areaIndex >= 0 && _areaIndex < _allArea.Length)
        {
            return _allArea[_areaIndex];
        }

        // 2. 인덱스에 문제가 있다면, 배열에 데이터가 있는지 확인
        if (_allArea.Length > 0)
        {
            // 데이터가 있으면 -> 첫번째 사냥터(기본) 설정
            return _allArea[0];
        }

        else
        {
            // 배열 자체가 비어있으면 반환X / 에러방지
            return null;
        }
    }

    // 헌터 이름생성 함수 / _randomJop를 확인 후 랜덤이름을 _nameList에 할당
    public void HunterRandomName()
    {
        // 버서커를 기본값으로 넣음
        List<string> hunterNameList = beserkerNames;

        if (_randomJop == HunterJop.Paladin)
        {
            hunterNameList = paladinNames;
        }

        else if (_randomJop == HunterJop.Ranger)
        {
            hunterNameList = rangerNames;
        }

        else if (_randomJop == HunterJop.Sorcerer)
        { 
            hunterNameList = sorcererNames;
        }

        int randomIndex = Random.Range(0, hunterNameList.Count);
        _nameList = hunterNameList[randomIndex];
    }
}
