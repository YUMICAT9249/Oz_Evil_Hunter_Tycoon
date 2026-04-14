using System;
using System.Collections.Generic;
using UnityEngine;

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

public class HunterData_PJS : MonoBehaviour, IUnit_YHJ
{
    public Action<float, float> OnHpChanged;
    public static Action OnHunterDie;

    #region 프로퍼티 외부용
    public float CurrentHP
    {
        get => _currentHP;
        set
        {
            _currentHP = value;
            OnHpChanged?.Invoke(_currentHP, _maxHP);
        }
    }
    public float MaxHP => _maxHP;
    public bool IsDead => _currentHP <= 0;
    public int Gold => _gold;
    #endregion

    [Header("기본 데이터 참조")]
    public UnitData_JBJ_PJS _unitData;

    [Header("헌터 기본 정보")]
    [SerializeField] public AreaType _areaType;     // 위치 확인용
    [SerializeField] public HunterJop _hunterJop;   // 헌터이름을 랜덤으로 생성할 직업타입
    [SerializeField] public string _hunterNameList; // 랜덤으로 생성된 이름을 담는 변수

    [Header("레벨 / 경험치")]
    [SerializeField] public int _currentLevel = 1;      // 현재 레벨
    [SerializeField] public float _currentExp;          // 현재 경험치
    [SerializeField] public float _maxExp = 100;        // 최대 경험치 초기값

    [Header("현재 체력")]
    [SerializeField] public float _currentHP = 100.0f;  // 현재 체력

    [Header("헌터 최종 스탯")]
    [SerializeField] public float _maxHP;           // 합산 최대HP
    [SerializeField] public float _damage;          // 합산 공격력
    [SerializeField] public float _defence;         // 합산 방어력
    [SerializeField] public float _criticalChance;  // 합산 치명타확률
    [SerializeField] public float _dodgeChance;     // 합산 회피확률
    [SerializeField] public float _attackCooldown;  // 합산 공격 속도
    [SerializeField] public float _moveSpeed;       // 합산 이동 속도

    [Header("탐지 / 공격 범위")]
    [SerializeField] public float _detectRange;     // 탐지 범위
    [SerializeField] public float _attackRange;     // 공격 사거리

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

    [Header("헌터 환생")]
    [SerializeField] public int _rebirthCount = 0;      // 환생 횟수
    [SerializeField] public float _rebirthBonus = 1.0f; // 환생 보너스 배율

    // 헌터 골드 / 재료 인벤토리 관련
    private Dictionary<object, int> _inventory = new Dictionary<object, int>();
    private int _gold = 0;

    // 헌터 장비 슬롯
    private object _weapon;
    private object _armor;
    private object _gloves;
    private object _boots;
    private object _ring;
    private object _necklace;

    // 직업별 헌터 이름
    private List<string> beserkerNames = new List<string> { "브란", "샤론", "세나" };
    private List<string> paladinNames = new List<string> { "카일", "알프", "홉스" };
    private List<string> rangerNames = new List<string> { "카이즈", "바레인", "크리샤" };
    private List<string> sorcererNames = new List<string> { "라글라스", "두아트린", "브리디도" };

    public static HunterData_PJS InfoHunter;

    private void Awake()
    {
        if (_unitData == null)
        {
            Debug.LogError("UnitData가 연결 안됨", gameObject);
            return;
        }

        // 직업이 NONE이면 기본값 버서커로 강제 설정
        if (_hunterJop == HunterJop.NONE)
        {
            _hunterJop = HunterJop.Berserker;
        }
        SettingHunterData(_hunterJop);
    }

    public void AddExp(int expAmount)
    {
        // EXP 증가 로직 (팀원이 필요에 따라 확장 가능)
        _currentExp += expAmount;
        while (_currentExp >= _maxExp) { LevelUp(); }
        Debug.Log($"[HunterData_PJS] {_hunterNameList}이(가) {expAmount} EXP 획득");
    }

    public void LevelUp()
    {
        _currentExp -= _maxExp; // 남은 경험치 유지
        _maxExp *= 2f;          // 필요경험치 복리 2배
        _currentLevel++;        // 레벨 증가

        FinalStats();   // 스탯 재계산
        Debug.Log($"{_hunterNameList} 레벨업! LV {_currentLevel}");
    }

    // 치료소 회복 함수
    public void Heal(float amount)
    {
        if (IsDead) return;
        _currentHP += amount;
        if (_currentHP > MaxHP) { _currentHP = _maxHP; }
        // HP변경 이벤트 -> UI 연결 및 갱신 
        OnHpChanged?.Invoke(_currentHP, _maxHP);
        Debug.Log($"{_hunterNameList}가 {amount}만큼 회복. 현재HP: {_currentHP}");
    }

    // 부활의 성소 부활 함수
    public void Revive()
    {
        if (!IsDead) return;

        _currentHP = _maxHP * 0.3f;
        // 부활 시 HP변경 이벤트 -> UI 연결 및 갱신 
        OnHpChanged?.Invoke(_currentHP, _maxHP);
        Debug.Log($"{_hunterNameList}가 부활");
    }

    // 헌터가 스폰된 후 헌터 데이터 세팅
    public void SettingHunterData(HunterJop jop)
    {
        _hunterJop = jop;

        switch (_hunterJop)
        {
            case HunterJop.Berserker:
                _detectRange = 1.5f;
                _attackRange = 0.3f;
                break;
            case HunterJop.Paladin:
                _detectRange = 1.5f;
                _attackRange = 0.3f;
                break;
            case HunterJop.Ranger:
                _detectRange = 1.5f;
                _attackRange = 1.0f;
                break;
            case HunterJop.Sorcerer:
                _detectRange = 1.5f;
                _attackRange = 1.0f;
                break;
        }
        HunterRandomName();
        RandomStats();
    }

    // 헌터 이름생성 함수 / _hunterJop을 확인 후 랜덤이름을 _nameList에 할당
    public void HunterRandomName()
    {
        // 버서커를 기본값으로 넣음
        List<string> hunterNameList = beserkerNames;

        if (_hunterJop == HunterJop.Paladin) { hunterNameList = paladinNames; }
        else if (_hunterJop == HunterJop.Ranger) { hunterNameList = rangerNames; }
        else if (_hunterJop == HunterJop.Sorcerer) { hunterNameList = sorcererNames; }
        _hunterNameList = hunterNameList[UnityEngine.Random.Range(0, hunterNameList.Count)];
    }

    // 스탯 뽑기 확률 함수
    private int GetRandomScore()
    {
        int randomScore = UnityEngine.Random.Range(0, 100);

        if (randomScore < 40) return 0;      // 40% 흰색
        else if (randomScore < 70) return 1; // 30% 파란색
        else if (randomScore < 90) return 2; // 20% 주황색
        else return 3;                       // 10% 보라색
    }

    // 랜덤 스탯 생성 + 최종 스탯 계산
    private void RandomStats()
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
        FinalStats();
        // 4. 현재 체력 초기화
        _currentHP = _maxHP;
    }

    // 스탯 점수 합산 / 등급 결정
    public void RankScore()
    {
        _totalScore = _hpScore + _damageScore + _defenceScore + _criticalChanceScore + _dodgeChanceScore + _attackCooldownScore + _moveSpeedScore;

        if (_totalScore <= 1) { _hunterRank = HunterRank.Normal; }
        else if (_totalScore <= 5) { _hunterRank = HunterRank.Rare; }
        else if (_totalScore <= 9) { _hunterRank = HunterRank.Superior; }
        else if (_totalScore <= 13) { _hunterRank = HunterRank.Heroic; }
        else if (_totalScore <= 17) { _hunterRank = HunterRank.Legendary; }
        else { _hunterRank = HunterRank.Ultimate; }
    }

    // 환생 조건 체크 후 실행 함수
    public void TryRebirth()
    {
        if (_currentLevel >= 100)
        {
            Rebirth();
        }
    }

    // 환생 로직
    public void Rebirth()
    {
        _rebirthCount++;
        _currentLevel = 1;
        _rebirthBonus = 1.0f + (_rebirthCount * 0.1f); // 1환생당 10% 추가 보너스 스탯 (복리x)
        FinalStats(); // 기존 등급에 환생 보너스만 계산
        _currentHP = _maxHP;

        Debug.Log($"{_hunterNameList} 환생. {_rebirthCount}회. {_rebirthBonus}배");
    }

    // 최종 스탯 계산 함수
    public void FinalStats()
    {
        _maxHP = AddStatsByScore(_unitData.maxHp, _hpScore) * _rebirthBonus;
        _damage = AddStatsByScore(_unitData.attackDamage, _damageScore) * _rebirthBonus;
        _defence = AddStatsByScore(_unitData.defence, _defenceScore) * _rebirthBonus;
        _criticalChance = AddStatsByScore(_unitData.criticalChance, _criticalChanceScore) * _rebirthBonus;
        _dodgeChance = AddStatsByScore(_unitData.dodgeChance, _dodgeChanceScore) * _rebirthBonus;
        _attackCooldown = AddAttackCooldownByScore(_unitData.attackCooldown, _attackCooldownScore) / _rebirthBonus;
        _moveSpeed = AddStatsByScore(_unitData.moveSpeed, _moveSpeedScore) * _rebirthBonus;

        ApplyEquipStats(); // 장비 수치 추가
    }

    // 점수별 스탯 추가 / 매개변수 사용 => 유지보수, 하나의 함수로 해결가능
    // 공격 쿨다운 제외
    private float AddStatsByScore(float baseValue, int score)
    {
        switch (score)
        {
            case 0: return baseValue * 1.0f;
            case 1: return baseValue * 1.1f;
            case 2: return baseValue * 1.2f;
            case 3: return baseValue * 1.3f;
        }
        return baseValue;
    }

    // 점수별 공격 쿨다운 줄임
    private float AddAttackCooldownByScore(float baseValue, int score)
    {
        switch (score)
        {
            case 0: return baseValue / 1.0f;
            case 1: return baseValue / 1.1f;
            case 2: return baseValue / 1.2f;
            case 3: return baseValue / 1.3f;
        }
        return baseValue;
    }

    // 최종 스탯계산에 추가하여 넣을 장비 수치
    private void ApplyEquipStats()
    { 
        // 장비 수치 나오면 채워넣을것
    }

    #region 헌터 인벤토리 연결용
    public void SetGold(int value) // 골드
    {
        _gold = value;
    }

    public void SetItem(object item) // 재료
    {
        if (item == null) return;

        if (_inventory.ContainsKey(item))
        {
            _inventory[item]++;
        }
        else
        {
            _inventory[item] = 1;
        }
    }

    public void SetWeapon(object weapon) // 무기 슬롯
    {
        _weapon = weapon;
        FinalStats();
    }

    public void SetArmor(object armor) // 갑옷 슬롯
    {
        _armor = armor;
        FinalStats();
    }

    public void SetGloves(object gloves) // 장갑 슬롯
    {
        _gloves = gloves;
        FinalStats();
    }

    public void SetBoots(object boots) // 부츠 슬롯
    {
        _boots = boots;
        FinalStats();
    }

    public void SetRing(object ring) // 반지 슬롯
    {
        _ring = ring;
        FinalStats();
    }

    public void SetNecklace(object necklace) // 목걸이 슬롯
    { 
        _necklace = necklace;
        FinalStats();
    }

    public Dictionary<object, int> GetInventory() // UI
    { 
        return _inventory;
    }
    #endregion

    #region Get 함수 => 최종 값만 반환
    public float GetMaxHP() => _maxHP;
    public float GetAttackDamage() => _damage;
    public float GetDefence() => _defence;
    public float GetCriticalChance() => _criticalChance;
    public float GetDodgeChance() => _dodgeChance;
    public float GetAttackCooldown() => Mathf.Max(0.25f, _attackCooldown);
    public float GetMoveSpeed() => _moveSpeed;
    #endregion
}
