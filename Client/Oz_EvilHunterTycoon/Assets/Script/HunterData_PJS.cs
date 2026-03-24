using UnityEngine;

// 헌터 클래스 열거형사용
// 헌터는 직업 별로 고유 이름, 스킬 등 있는데 문자열로 나누기 위한 작업
public enum HunterJop
{
    Berserker,
    Paladin,
    Ranger,
    Sorcerer
}

public class HunterData_PJS : MonoBehaviour
{
    [Header("헌터 기본 정보")]
    [SerializeField] private string _hunterName;
    [SerializeField] private HunterJop _hunterJop;
    [SerializeField] private AreaType _currentArea;

    [Header("헌터 상세 스텟")]
    [SerializeField] public int _currentLevel = 1;             // 현재 레벨
    [SerializeField] public int _maxLevel = 100;               // 최대 레벨
    [SerializeField] public float _currentHP = 100.0f;         // 현재 체력
    [SerializeField] public float _maxHP = 100.0f;             // 최대 체력
    [SerializeField] public float _baseDamage = 10.0f;         // 기본 공격력
    [SerializeField] public float _baseDefence = 10.0f;        // 기본 방어력
    [SerializeField] public float _baseAttackSpeed = 1.0f;     // 기본 공격속도
    [SerializeField] public float _baseCriticalChance = 1.0f;  // 기본 치명타확률
    [SerializeField] public float _baseDodgeChance = 1.0f;     // 기본 회피확률

    [Header("헌터 이동 및 거리 설정")]
    [SerializeField] private float _baseAttackRange = 1.0f; // 공격 사거리
    [SerializeField] private float _baseDetectRange = 3.0f; // 감지 거리
    [SerializeField] private float _moveSpeed = 2.0f;       // 이동 속도

    #region 프로퍼티 외부 참조용 (사용이유 => 변수에 보안장치와 자동화)
    // 이름 설정 및 반환
    public string Name
    {
        get { return _hunterName; }
        set { _hunterName = value; }
    }
    // 직업 설정 및 반환
    public HunterJop Jop
    {
        get { return _hunterJop; }
        set { _hunterJop = value; }
    }
    // 현재 구역 설정 및 반환 (매니저에서 이동시 사용)
    public AreaType _areaType
    {
        get { return _currentArea; }
        set { _currentArea = value; }
    }

    // 레벨 설정
    public int Level
    {
        get { return _currentLevel; }
        set
        {
            if (value <= _maxLevel)
            {
                _currentLevel = value;
            }
            else
            {
                _currentLevel = _maxLevel;
            }
        }
    }
    // 체력 설정
    public float HP
    {
        get { return _currentHP; }
        set 
        { 
            _currentHP = value;
            if (_currentHP > _maxHP)
            { 
                _currentHP = _maxHP;
            }
            if (_currentHP < 0)
            {
                _currentHP = 0;
            }
        }
    }
    // 공격력 설정
    public float Damage
    {
        get { return _baseDamage; }
        set { _baseDamage = value; }
    }
    // 방어력 설정
    public float Defence
    {
        get { return _baseDefence; }
        set { _baseDefence = value; }
    }
    // 공격속도
    public float AttackSpeed
    {
        get { return _baseAttackSpeed; }
        set { _baseAttackSpeed = value; }
    }
    // 치명타확률
    public float CriticalChance
    {
        get { return _baseCriticalChance; }
        set { _baseCriticalChance = value; }
    }
    // 회피확률
    public float DodgeChance
    {
        get { return _baseDodgeChance; }
        set { _baseDodgeChance = value; }
    }

    // 공격 사거리
    public float AttackRange
    {
        get { return _baseAttackRange; }
        set { _baseAttackRange = value; }
    }
    // 감지 거리
    public float DetectRange
    {
        get { return _baseDetectRange; }
        set { _baseDetectRange = value; }
    }
    // 이동 속도
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }
    #endregion
}
