using UnityEngine;
using System.Collections.Generic;

// 헌터 데이터 + 수치 계산식 스크립트

// 헌터 클래스 열거형사용
// 헌터는 직업 별로 고유 이름, 스킬 등 있는데 문자열로 나누기 위한 작업
public enum HunterJop
{
    NONE,
    Berserker,
    Paladin,
    Ranger,
    Sorcerer
}

public enum HunterRank
{ 
    NONE,
    Normal,
    Rare,
    Superior,
    Heroic,
    Legendary,
    Ultimate
}

public class HunterData_PJS : MonoBehaviour
{
    [Header("기본 데이터 참조")]
    public UnitData_JBJ_PJS _unitData;

    [Header("헌터 기본 정보")]
    [SerializeField] public AreaType _areaType;     // 위치 확인용
    [SerializeField] public HunterJop _hunterJop;   // 헌터이름을 랜덤으로 생성할 직업타입
    [SerializeField] public string _hunterNameList;       // 랜덤으로 생성된 이름을 담는 변수

    [Header("헌터 상세 스탯")]
    [SerializeField] public int _currentLevel = 1;      // 현재 레벨
    [SerializeField] public float _currentHP = 100.0f;  // 현재 체력

    [Header("헌터 최종 스탯")]
    [SerializeField] public float _maxHP;           // 합산 최대HP
    [SerializeField] public float _damage;          // 합산 공격력
    [SerializeField] public float _defence;         // 합산 방어력
    [SerializeField] public float _criticalChance;  // 합산 치명타확률
    [SerializeField] public float _dodgeChance;     // 합산 회피확률
    [SerializeField] public float _attackCooldown;  // 합산 공격 속도
    [SerializeField] public float _moveSpeed;       // 합산 이동 속도

    [Header("스탯 점수 (0:하급(흰색) 1:중급(파란색) 2:상급(주황색) 3:최상급(보라색))")]
    [SerializeField] public int _hpScore;             // HP 등급
    [SerializeField] public int _damageScore;         // 공격력 등급
    [SerializeField] public int _defenceScore;        // 방어력 등급
    [SerializeField] public int _criticalChanceScore; // 치명타확률 등급
    [SerializeField] public int _dodgeChanceScore;    // 회피확률 등급
    [SerializeField] public int _attackCooldownScore; // 공격속도 등급
    [SerializeField] public int _moveSpeedScore;      // 이동속도 등급
    
    [Header("헌터 등급 결과")]
    [SerializeField] public int _totalScore;
    [SerializeField] public HunterRank _hunterRank;

    // 직업별 헌터 이름
    private List<string> beserkerNames = new List<string> { "브란", "샤론", "세나" };
    private List<string> paladinNames = new List<string> { "카일", "알프", "홉스" };
    private List<string> rangerNames = new List<string> { "카이즈", "바레인", "크리샤" };
    private List<string> sorcererNames = new List<string> { "라글라스", "두아트린", "브리디도" };

    private void Awake()
    {
        if (_unitData == null)
        {
            Debug.LogError("UnitData가 연결 안됨", gameObject);
            return;
        }
        HunterRandomJop();
        HunterRandomName();
        RandomStats();
    }

    // 헌터 직업 랜덤 생성
    public void HunterRandomJop()
    {
        int random = Random.Range(1, 5);
        _hunterJop = (HunterJop)random;
    }

    // 헌터 이름생성 함수 / _hunterJop을 확인 후 랜덤이름을 _nameList에 할당
    public void HunterRandomName()
    {
        // 버서커를 기본값으로 넣음
        List<string> hunterNameList = beserkerNames;

        if (_hunterJop == HunterJop.Paladin)
        {
            hunterNameList = paladinNames;
        }

        else if (_hunterJop == HunterJop.Ranger)
        {
            hunterNameList = rangerNames;
        }

        else if (_hunterJop == HunterJop.Sorcerer)
        {
            hunterNameList = sorcererNames;
        }

        int randomIndex = Random.Range(0, hunterNameList.Count);
        _hunterNameList = hunterNameList[randomIndex];
    }

    // 스탯 뽑기 확률 함수
    private int GetRandomScore()
    {
        int randomScore = Random.Range(0, 100);

        if (randomScore < 40) { return 0; }      // 40% 흰색
        else if (randomScore < 70) { return 1; } // 30% 파란색
        else if (randomScore < 90) { return 2; } // 20% 주황색
        else { return 3; }                       // 10% 보라색
    }

    // 점수별 스탯 추가 / 매개변수 사용 => 유지보수, 하나의 함수로 해결가능
    // 공격 쿨다운 제외
    public float AddStatsByScore(float baseValue, int score)
    {
        switch (score)
        { 
            case 0: 
                return baseValue * 1.0f;
            case 1:
                return baseValue * 1.1f;
            case 2:
                return baseValue * 1.2f;
            case 3:
                return baseValue * 1.3f;
        }
        return baseValue;
    }

    // 점수별 공격 쿨다운 줄임
    public float AddAttackCooldownByScore(float baseValue, int score)
    {
        switch (score)
        {
            case 0:
                return baseValue / 1.0f;
            case 1:
                return baseValue / 1.1f;
            case 2:
                return baseValue / 1.2f;
            case 3:
                return baseValue / 1.3f;
        }
        return baseValue;
    }


    // 스탯 점수 합산 / 등급 결정
    public void RankScore()
    {
        _totalScore = _hpScore + _damageScore + _defenceScore + _criticalChanceScore + _dodgeChanceScore + _attackCooldownScore + _moveSpeedScore;

        if (_totalScore <= 1)
        {
            _hunterRank = HunterRank.Normal;
        }
        else if (_totalScore <= 5)
        {
            _hunterRank = HunterRank.Rare;
        }
        else if (_totalScore <= 9)
        {
            _hunterRank = HunterRank.Superior;
        }
        else if (_totalScore <= 13)
        {
            _hunterRank = HunterRank.Heroic;
        }
        else if (_totalScore <= 17)
        {
            _hunterRank = HunterRank.Legendary;
        }
        else
        {
            // 확장 개념으로 넣어둠(현시점에서 사용x)
            _hunterRank = HunterRank.Ultimate;
        }
    }

    // 랜덤 스탯 생성 + 최종 스탯 계산
    public void RandomStats()
    {
        // 1. 점수 생성
        _hpScore = GetRandomScore();
        _damageScore = GetRandomScore();
        _defenceScore = GetRandomScore();
        _criticalChanceScore = GetRandomScore();
        _dodgeChanceScore = GetRandomScore();
        _attackCooldownScore = GetRandomScore();
        _moveSpeedScore = GetRandomScore();

        // 2. 등급 계산
        RankScore();

        // 3. 최종 스탯 계산
        _maxHP = AddStatsByScore(_unitData.maxHp, _hpScore);
        _damage = AddStatsByScore(_unitData.attackDamage, _damageScore);
        _defence = AddStatsByScore(_unitData.defence, _defenceScore);
        _criticalChance = AddStatsByScore(_unitData.criticalChance, _criticalChanceScore);
        _dodgeChance = AddStatsByScore(_unitData.dodgeChance, _dodgeChanceScore);
        _attackCooldown = AddAttackCooldownByScore(_unitData.attackCooldown, _attackCooldownScore);
        _moveSpeed = AddStatsByScore(_unitData.moveSpeed, _moveSpeedScore);

        // 4. 현재 체력 초기화
        _currentHP = _maxHP;
    }

    #region Get 함수 => 최종 값만 반환
    public float GetMaxHP()
    {
        return _maxHP;
    }

    public float GetAttackDamage()
    {
        return _damage;
    }

    public float GetDefence()
    {
        return _defence;
    }

    public float GetCriticalChance()
    {
        return _criticalChance;
    }

    public float GetDodgeChance()
    {
        return _dodgeChance;
    }

    public float GetAttackCooldown()
    {
        return _attackCooldown;
    }

    public float GetMoveSpeed()
    {
        return _moveSpeed;
    }
    #endregion
}
