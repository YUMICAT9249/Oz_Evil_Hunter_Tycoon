using UnityEngine;
using System.Collections;

// 헌터 행동(어디로 이동/어떻게 공격) 스크립트

public class HunterController_PJS : MonoBehaviour
{
    // [1] 헌터 상태
    private enum HunterState
    {
        Idle, Move, Attack
    }

    [SerializeField] private HunterState _currentState = HunterState.Idle;
    [SerializeField] private AreaType _areaCheck; // 이전 지역 저장용

    // [2] 참조
    [Header("헌터 데이터 참조")]
    [SerializeField] private UnitData_JBJ_PJS _unitData;
    [SerializeField] private HunterData_PJS _hunterData;
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider2D _targetBox;   // 현재 이동 영역

    private HunterBattle_PJS _battle;
    // [3] 이동 타겟 관련
    private GameObject _targetMonster;  // 현재 타겟
    private Vector2 _targetPosition;    // 이동 목적지
    private float _lookTargetX;
    // [4] 내부 상태값
    private float _lastAttackTime;
    private float _idleTime = 1.0f;
    private bool _isForcedMove = false;

    // [5] 초기화
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _battle = GetComponent<HunterBattle_PJS>();

        if (_unitData == null)
        {
            Debug.LogError("UnitData 연결 없음", gameObject);
        }
        if (_hunterData == null)
        {
            Debug.LogError("HunterData 연결 없음", gameObject);
        }
    }

    void Start()
    {
        // 매니저에 등록
        if (HunterManager_PJS.Instance != null)
        {
            HunterManager_PJS.Instance._activeHunters.Add(this);
        }
        // 기본 지역 저장
        _areaCheck = _hunterData._areaType;
        // 초기 위치 설정
        UpdateLocation();
        // 행동 후프 시작(1회)
        StartCoroutine(HunterActionCenterLoop());
    }

    void Update()
    {
        // 지역 변경 감지
        AreaCheck(); 
    }

    // [6] 지역 변경 감지 (변경될 때만 실행)
    private void AreaCheck()
    {
        if (_hunterData == null) return;

        if (_areaCheck != _hunterData._areaType)
        {
            _areaCheck = _hunterData._areaType;
            UpdateLocation();
        }
    }

    // [7] 위치 갱신 (유저 명령 들어왔을 때 실행)
    public void UpdateLocation()
    {
        // 강제 이동 ON
        _isForcedMove = true;

        if (HunterManager_PJS.Instance != null)
        {
            // 새로운 지역 설정
            _targetBox = HunterManager_PJS.Instance.GetAreaCollider(_hunterData._areaType);
        }

        if (_targetBox != null)
        {
            // 새로운 목적지 설정
            RandomPos();
        }
    }

    // [8] 행동 중앙 제어(메인)
    IEnumerator HunterActionCenterLoop()
    {
        BoxCollider2D currentBox = _targetBox;

        while (true)
        {
            // 1. 유저 명령 최우선 처리
            if (_isForcedMove)
            {
                // 기존 타겟 제거
                _targetMonster = null; 

                _currentState = HunterState.Move;
                yield return StartCoroutine(HunterMoveLoop());

                // 이동 완료
                _isForcedMove = false; 
                continue;
            }
            // 2. 타겟 탐색
            FindTarget();

            // 3. 타겟 없음 → 이동
            if (_targetMonster == null)
            {
                _currentState = HunterState.Move;
                yield return StartCoroutine(HunterMoveLoop());

                _currentState = HunterState.Idle;
                _animator.SetBool("IsMoving", false);
                yield return new WaitForSeconds(_idleTime);
            }
            // 4. 타겟 있음
            else
            {
                float distance = Vector2.Distance(transform.position, _targetMonster.transform.position);

                // 공격 범위 안
                if (distance <= _unitData.attackRange)
                {
                    _currentState = HunterState.Attack;
                    _animator.SetBool("IsMoving", false);
                    yield return StartCoroutine(HunterAttackLoop());
                }
                // 범위 밖 → 추격
                else
                {
                    yield return StartCoroutine(HunterFollowLoop());
                }
            }
        }
    }

    // [9] 이동
    IEnumerator HunterMoveLoop()
    {
        if (_targetBox == null) yield break;

        BoxCollider2D currentBox = _targetBox;

        if (!_isForcedMove)
        {
            RandomPos();
        }

        _lookTargetX = _targetPosition.x;
        LookAt();
        _animator.SetBool("IsMoving", true);

        while (Vector2.Distance(transform.position, _targetPosition) > 0.1f)
        {
            // 주변에 몬스터가 있는지 확인
            FindTarget();

            // 몬스터를 찾았다면
            if (_targetMonster != null)
            {
                // 이동루프 탈출 -> HunterActionCenterLoop()로 복귀
                _animator.SetBool("IsMoving", false);
                // 상태를 추격으로 변경
                _currentState = HunterState.Move;
                yield return StartCoroutine(HunterFollowLoop());
                yield break;
            }

            // 지역 변경 감지
            if (_targetBox != currentBox)
            {
                yield break;
            }

            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _hunterData.GetMoveSpeed() * Time.deltaTime);
            yield return null;
        }

        _animator.SetBool("IsMoving", false);
    }

    // [10] 추격
    IEnumerator HunterFollowLoop()
    {
        _currentState = HunterState.Move;
        _animator.SetBool("IsMoving", true);

        while (_targetMonster != null)
        {
            float distance = Vector2.Distance(transform.position, _targetMonster.transform.position);
            if (distance <= _unitData.attackRange)
            {
                _animator.SetBool("IsMoving", false);
                yield return StartCoroutine(HunterAttackLoop());
                break;
            }

            _lookTargetX = _targetMonster.transform.position.x;
            LookAt();

            transform.position = Vector2.MoveTowards(transform.position, _targetMonster.transform.position, _hunterData.GetMoveSpeed() * Time.deltaTime);
            yield return null;
        }

        _animator.SetBool("IsMoving", false);
    }

    // [11] 공격
    IEnumerator HunterAttackLoop()
    {
        while (_targetMonster != null)
        {
            float cooldown = _hunterData.GetAttackCooldown();

            if (Time.time >= _lastAttackTime + _hunterData.GetAttackCooldown())
            {
                _lookTargetX = _targetMonster.transform.position.x;
                LookAt();

                // 애니메이션 공격 쿨다운 조절(공격속도 조절)
                float baseCooldown = _unitData.attackCooldown;
                _animator.speed = baseCooldown / cooldown;
                _animator.SetTrigger("Attack");
                
                // 데미지 처리
                if (_battle != null)
                {
                    _battle.GiveDamage(_targetMonster);
                }
                _lastAttackTime = Time.time;

                yield return new WaitForSeconds(_hunterData.GetAttackCooldown());
                // 공격 후 애니메이션 속도 복구
                _animator.speed = 1.0f;
            }
            else
            {
                yield return null;
            }

            FindTarget();
        }
    }

    // [12] 몬스터 찾기
    private void FindTarget()
    {
        // 1. 기존 타겟이 살아있는지 체크
        if (_targetMonster != null)
        {
            // 몬스터 사망시 해제
            if (!_targetMonster.activeInHierarchy)
            {
                _targetMonster = null;
            }
            // 범위 밖이면 타겟 해제
            else if (!_targetBox.OverlapPoint(_targetMonster.transform.position))
            {
                _targetMonster = null;
            }
            // 범위 안이면 타겟 유지
            else
            {
                return;
            } 
        }
        // 2. 새로운 몬스터 탐지
        GameObject monster = GameObject.FindWithTag("Monster");

        if (monster == null)
        {
            _targetMonster = null;
            return;
        }

        // 구역 체크
        if (!_targetBox.OverlapPoint(monster.transform.position))
        {
            _targetMonster = null;
            return;
        }

        // 거리 체크
        float distance = Vector2.Distance(transform.position, monster.transform.position);
        if (distance > _unitData.detectRange)
        {
            _targetMonster = null;
            return;
        }
        _targetMonster = monster;
    }

    // [13] 방향 전환
    private void LookAt()
    {
        if (_lookTargetX > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        { 
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // [14] 랜덤 위치 생성
    private void RandomPos()
    {
        Bounds areaBounds = _targetBox.bounds;

        while (true)
        {
            float x = Random.Range(areaBounds.min.x, areaBounds.max.x);
            float y = Random.Range(areaBounds.min.y, areaBounds.max.y);
            _targetPosition = new Vector2(x, y);

            if (_targetBox.OverlapPoint(_targetPosition))
                break;
        }
    }

    // [15] 헌터 사망 처리 함수
    public void HunterDie()
    { 
        StopAllCoroutines();
        _animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
    }

    // [16] 마을 귀환 함수
    public void ReturnVillage()
    {
        _hunterData._areaType = AreaType.Village;
        BoxCollider2D villageBox = HunterManager_PJS.Instance.GetAreaCollider(AreaType.Village);

        if (villageBox != null)
        {
            transform.position = villageBox.bounds.center;
        }

        // 반투명 알파 값 복구
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        { 
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
        }

        // HP 최대치 + 부활처리
        _hunterData._currentHP = _hunterData.GetMaxHP();
        GetComponent<Collider2D>().enabled = true;
        UpdateLocation();
    }
}