using UnityEngine;
using System.Collections;

// 헌터 행동(어디로 이동/어떻게 공격) 스크립트

public class HunterController_PJS : BaseWorldObject_KJG
{
    // [1] 헌터 상태
    private enum HunterState
    {
        Idle, Move, Attack, Die
    }

    [SerializeField] private HunterState _currentState = HunterState.Idle;
    [SerializeField] private AreaType _areaCheck; // 이전 지역 저장용

    // [2] 참조
    [Header("이동 영역")]
    [SerializeField] private BoxCollider2D _targetBox;   // 현재 이동 영역

    // 최적화를 위한 변수처리 (TryGetComponent, FindWithTag 제거)
    private HunterData_PJS _hunterData;
    private HunterSkill_PJS _hunterSkill;
    private Battle_JBJ_PJS _battle;
    private Animator _animator;
    private Collider2D _hunterCollider;
    private SpriteRenderer _spriteRenderer;

    // [3] 이동 타겟 관련
    private Battle_JBJ_PJS _targetBattle;
    private GameObject _targetMonster;  // 현재 타겟
    private Vector2 _targetPosition;    // 이동 목적지
    private float _lookTargetX;

    // [4] 내부 상태값
    private float _lastAttackTime;
    private float _idleTime = 1.0f;
    private bool _isForcedMove = false;

    // 몬스터 탐색 (FindWithTag 제거)
    private Collider2D[] _detectMonster = new Collider2D[20];

    protected override void Awake()
    {
        base.Awake();
        // 캐싱 - 최적화
        _hunterData = GetComponent<HunterData_PJS>();
        _hunterSkill = GetComponent<HunterSkill_PJS>();
        _battle = GetComponent<Battle_JBJ_PJS>();
        _animator = GetComponent<Animator>();
        _hunterCollider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // [5] 초기화
    protected override void Start()
    {
        StartCoroutine(ManagerWaiting());
    }

    void Update()
    {
        // 지역 변경 감지
        AreaCheck();
    }

    // [7] 위치 갱신 (매니저가 소환/이동 시 직접 호출)
    public void SetArea(BoxCollider2D newArea)
    {
        _targetBox = newArea;
        _isForcedMove = true; // 강제 이동 ON

        if (_targetBox != null)
        {
            RandomPos(); // 새로운 목적지 설정
        }
    }

    // [15] 헌터 사망 처리 함수
    public void HunterDie()
    {
        StopAllCoroutines();
        _currentState = HunterState.Die;

        _animator.SetTrigger("Die");
        _animator.SetBool("IsMoving", false);
        _animator.speed = 1.0f;

        _hunterCollider.enabled = false;
        // 사망 이벤트
        HunterData_PJS.OnHunterDie?.Invoke();
        if (TryGetComponent(out IUnit_YHJ unit))
        {
            EventBus_YHJ.RequestInteract?.Invoke(gameObject, unit);
        }
    }

    // [6] 지역 변경 감지 (변경될 때만 실행)
    private void AreaCheck()
    {
        if (_hunterData != null && _areaCheck != _hunterData._areaType)
        {
            // 구역이 변경되면 "외부 매니저"에서 SetArea 호출
            _areaCheck = _hunterData._areaType;
        }
    }

    // [12] 몬스터 찾기
    private void FindTarget()
    {
        if (_targetBox == null) return;

        // 1. 기존 타겟이 살아있는지 체크
        if (_targetMonster != null)
        {
            // 몬스터 사망시 해제 / 범위 밖이면 타겟 해제
            if (!_targetMonster.activeInHierarchy || !_targetBox.OverlapPoint(_targetMonster.transform.position))
            {
                _targetMonster = null;
                _targetBattle = null;
            }
            // 범위 안이면 타겟 유지
            else return;
        }
        // 2. 새로운 몬스터 탐지 (특정 태그(Monster)만 탐지)
        int count = Physics2D.OverlapCircleNonAlloc
            (
                transform.position,
                _hunterData._detectRange,
                _detectMonster
            );
        float closestDistance = float.MaxValue;
        GameObject closestMonster = null;
        Battle_JBJ_PJS closestBattle = null;

        for (int i = 0; i < count; i++)
        {
            if (_detectMonster[i] == null) continue;

            if (_detectMonster[i].CompareTag("Monster"))
            {
                if (_targetBox.OverlapPoint(_detectMonster[i].transform.position))
                {
                    float distance = Vector2.Distance(transform.position, _detectMonster[i].transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMonster = _detectMonster[i].gameObject;
                        closestBattle = _detectMonster[i].GetComponent<Battle_JBJ_PJS>();
                    }
                }
            }
        }
        _targetMonster = closestMonster;
        _targetBattle = closestBattle;
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
        if (_targetBox == null) return;
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

    // [16] 마을 귀환 함수
    public void ReturnVillage(BoxCollider2D villageBox)
    {
        _hunterData._areaType = AreaType.Village;
        _areaCheck = AreaType.Village;

        if (villageBox != null)
        {
            transform.position = villageBox.bounds.center;
            _targetBox = villageBox; // 구역 갱신
        }

        // 반투명 알파 값 복구
        if (_spriteRenderer != null)
        {
            Color color = _spriteRenderer.color;
            color.a = 1f;
            _spriteRenderer.color = color;
        }

        // HP 최대치 + 부활처리
        _hunterData.CurrentHP = _hunterData.GetMaxHP();
        _hunterCollider.enabled = true;
        _currentState = HunterState.Idle;

        SetArea(_targetBox); // 위치갱신 로직 실행
        StopAllCoroutines();
        StartCoroutine(HunterActionCenterLoop());
    }

    // [8] 행동 중앙 제어(메인)
    IEnumerator HunterActionCenterLoop()
    {
        while (true)
        {
            // 1. 유저 명령 최우선 처리
            if (_isForcedMove)
            {
                _targetMonster = null; // 기존 타겟 제거
                _targetBattle = null;

                _currentState = HunterState.Move;
                yield return StartCoroutine(HunterMoveLoop());

                _isForcedMove = false; // 이동 완료
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
                if (_hunterData)
                {
                    float distance = Vector2.Distance(transform.position, _targetMonster.transform.position);
                    // 공격 범위 안
                    if (distance <= _hunterData._attackRange)
                    {
                        _currentState = HunterState.Attack;
                        _animator.SetBool("IsMoving", false);
                        yield return StartCoroutine(HunterAttackLoop());
                    }
                    // 범위 밖 → 추격
                    else
                    {
                        _currentState = HunterState.Move;
                        yield return StartCoroutine(HunterFollowLoop());
                    }
                }
            }
        }
    }

    // [9] 이동
    IEnumerator HunterMoveLoop()
    {
        if (_targetBox == null) yield break;

        if (!_isForcedMove) { RandomPos(); }

        _lookTargetX = _targetPosition.x;
        LookAt();
        _animator.SetBool("IsMoving", true);

        while (Vector2.Distance(transform.position, _targetPosition) > 0.1f)
        {
            FindTarget(); // 주변에 몬스터가 있는지 확인

            // 몬스터를 찾았다면
            if (_targetMonster != null)
            {
                // 이동루프 탈출 -> HunterActionCenterLoop()로 복귀
                _animator.SetBool("IsMoving", false);
                yield break;
            }

            // 이동속도 적용
            if (_hunterData)
            {
                transform.position = Vector2.MoveTowards
                    (
                        transform.position,
                        _targetPosition,
                        _hunterData.GetMoveSpeed() * Time.deltaTime
                    );
            }
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

            if (distance <= _hunterData._attackRange)
            {
                _animator.SetBool("IsMoving", false);
                yield break;
            }

            _lookTargetX = _targetMonster.transform.position.x;
            LookAt();

            transform.position = Vector2.MoveTowards
                (
                    transform.position,
                    _targetMonster.transform.position,
                    _hunterData.GetMoveSpeed() * Time.deltaTime
                );
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
    }

    // [11] 공격
    IEnumerator HunterAttackLoop()
    {
        while (_targetMonster != null)
        {
            // 타겟 죽었으면 즉시 종료
            if (!_targetMonster.activeInHierarchy) yield break;

            float currentDistance = Vector2.Distance(transform.position, _targetMonster.transform.position);
            if (currentDistance > _hunterData._attackRange)
            {
                _animator.speed = 1.0f;
                yield break;
            }

            float cooldown = _hunterData.GetAttackCooldown();

            if (Time.time >= _lastAttackTime + cooldown)
            {
                _lookTargetX = _targetMonster.transform.position.x;
                LookAt();

                // 애니메이션 공격 쿨다운 조절(공격속도 조절)
                _animator.speed = _hunterData._unitData.attackCooldown / cooldown;

                // 헌터 스킬 사용 시도
                if (_hunterSkill != null && _targetBattle != null)
                {
                    _hunterSkill.UseSkill(_targetBattle);
                }

                _animator.SetTrigger("Attack");

                // 데미지 처리 (전투 스크립트 호출)
                if (_battle != null && _targetBattle != null)
                {
                    _battle.GiveDamage(_targetBattle);
                }
                _lastAttackTime = Time.time;

                yield return new WaitForSeconds(cooldown);
                // 공격 후 애니메이션 속도 복구
                _animator.speed = 1.0f;
            }
            else
            {
                yield return null;
            }
            FindTarget();
        }
        _animator.speed = 1.0f;
    }

    #region Manager 초기화 웨이팅
    IEnumerator ManagerWaiting()
    {
        yield return null;

        base.Start();
        // 시작 시 현재 구역을 가져옴
        if (_hunterData != null)
        {
            // 기본 지역 저장
            _areaCheck = _hunterData._areaType;
        }
        // 행동 루프 시작(1회)
        StartCoroutine(HunterActionCenterLoop());
    }
    #endregion

    #region UI
    void OnMouseDown()
    {
        if (_hunterData == null) return;
        HunterData_PJS.InfoHunter = _hunterData;
    }
    #endregion
}