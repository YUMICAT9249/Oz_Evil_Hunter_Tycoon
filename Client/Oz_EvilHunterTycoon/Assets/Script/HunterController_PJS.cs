using UnityEngine;
using System.Collections;

// 헌터 행동(어디로 이동/어떻게 공격) 스크립트

public class HunterController_PJS : BaseWorldObject_KJG, OnClick_KSH
{
    // 헌터 상태
    private enum HunterState
    {
        Idle, Move, Attack, Die
    }

    [SerializeField] private HunterState _currentState = HunterState.Idle;
    [SerializeField] private AreaType _areaCheck; // 이전 지역 저장용

    [Header("이동 영역")]
    [SerializeField] private BoxCollider2D _targetBox;   // 현재 이동 영역

    [Header("원거리 기본공격")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private float projectileSpeed = 5f;

    [Header("발사 위치")]
    [SerializeField] private Transform firePoint;

    // 최적화를 위한 변수처리 (TryGetComponent, FindWithTag 제거)
    private HunterData_PJS _hunterData;
    private HunterSkill_PJS _hunterSkill;
    private Battle_JBJ_PJS _battle;
    private Animator _animator;
    private Collider2D _hunterCollider;
    private SpriteRenderer _spriteRenderer;

    // 이동 타겟 관련
    private Battle_JBJ_PJS _targetBattle;
    private GameObject _targetMonster;  // 현재 타겟
    private Vector2 _targetPosition;    // 이동 목적지
    private float _lookTargetX;

    // 건물 상호작용 관련
    private Transform _buildingTarget;      // 건물 타겟
    private BuildingType_YHJ _buildingType; // 건물 타입

    // 내부 상태값
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

    // 초기화
    protected override void Start()
    {
        StartCoroutine(ManagerWaiting());
    }

    void Update()
    {
        // 지역 변경 감지
        AreaCheck();
    }

    // 위치 갱신 (매니저가 소환/이동 시 직접 호출)
    public void SetArea(BoxCollider2D newArea)
    {
        _targetBox = newArea;
        _isForcedMove = true; // 강제 이동 ON

        if (_targetBox != null)
        {
            RandomPos(); // 새로운 목적지 설정
        }
    }

    // 헌터 사망 처리 함수
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

    // 마을 귀환 함수
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

    // 발사체 함수
    public void FireProjectile()
    {
        if (_targetMonster == null) return;
        GameObject projectilePrefab = null;
        // 직업별 발사체 프리팹
        if (_hunterData._hunterJop == HunterJop.Ranger)
        {
            projectilePrefab = arrowPrefab;
        }
        else if (_hunterData._hunterJop == HunterJop.Sorcerer)
        {
            projectilePrefab = fireBallPrefab;
        }

        if (projectilePrefab == null) return;

        Vector2 direction = (_targetMonster.transform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate
            (
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );
        // 직업별 방향 처리
        if (_hunterData._hunterJop == HunterJop.Ranger)
        {
            projectile.transform.up = -direction;
        }
        else
        {
            projectile.transform.up = direction;
        }
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * projectileSpeed;
        }
        // 본인 충돌 무시
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();

        if (myCollider != null && projectileCollider != null)
        {
            Physics2D.IgnoreCollision(projectileCollider, myCollider);
        }
    }

    // 지역 변경 감지 (변경될 때만 실행)
    private void AreaCheck()
    {
        if (_hunterData != null && _areaCheck != _hunterData._areaType)
        {
            // 구역이 변경되면 "외부 매니저"에서 SetArea 호출
            _areaCheck = _hunterData._areaType;
        }
    }

    // 몬스터 찾기
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

    // 헌터 좌우 방향 전환
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

    // 이동할 위치 랜덤 생성
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

    // 헌터 본인 위치 기준으로 Area 찾기
    private BoxCollider2D FindAreaPosition()
    {
        Collider2D[] find = Physics2D.OverlapPointAll(transform.position);
        for (int i = 0; i < find.Length; i++)
        {
            if (find[i] is BoxCollider2D box)
            { 
                return box;
            }
        }
        return null;
    }

    #region 건물에서 연결되어야할 함수 / 호재님 인터페이스 필요
    private void BuildingInteraction()
    {
        if (_buildingTarget == null) return;

        // ★ 2026-04-16 YHJ: 헌터가 건물에 도착하면 YHJ 건물 상호작용 이벤트로 넘겨 실제 Interact 흐름을 연결
        if (!TryGetComponent(out IUnit_YHJ unit))
        {
            // ★ 2026-04-16 YHJ: 헌터가 IUnit_YHJ를 못 찾는 경우 건물 상호작용을 시도하지 않도록 안전 처리
            Debug.LogWarning("IUnit_YHJ 없음");
            _buildingTarget = null;
            return;
        }

        // ★ 2026-04-16 YHJ: BuildingInteractionReceiver_YHJ가 RequestInteract를 받아
        // IBuildingInteraction_YHJ.CanInteract/Interact를 호출하므로 헌터 쪽에서는 타겟과 유닛만 전달하면 됨
        EventBus_YHJ.RequestInteract?.Invoke(_buildingTarget.gameObject, unit);
        _buildingTarget = null;
    }
    #endregion

    // 행동 중앙 제어(메인)
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

    // 이동 코루틴
    IEnumerator HunterMoveLoop()
    {
        if (_targetBox == null) yield break;

        if (!_isForcedMove) { RandomPos(); }

        _lookTargetX = _targetPosition.x;
        LookAt();
        _animator.SetBool("IsMoving", true);

        while (Vector2.Distance(transform.position, _targetPosition) > 0.1f)
        {
            // 건물 도착 체크
            if (_buildingTarget != null)
            {
                float distance = Vector2.Distance(transform.position, _buildingTarget.position);
                if (distance < 0.2f)
                {
                    _animator.SetBool("IsMoving", false);
                    BuildingInteraction();
                    yield break;
                }
            }
            FindTarget(); // 주변에 몬스터가 있는지 확인

            // 몬스터를 찾았다면
            if (_targetMonster != null)
            {
                // 이동루프 탈출 -> HunterActionCenterLoop()로 복귀
                _animator.SetBool("IsMoving", false);
                yield break;
            }
            transform.position = Vector2.MoveTowards
                (
                    transform.position,
                    _targetPosition,
                    _hunterData.GetMoveSpeed() * Time.deltaTime
                );
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
    }

    // 추격 코루틴
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

    // 공격 코루틴
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
                if (Manager_KJG.Audio != null)
                {
                    if (_hunterData._hunterJop == HunterJop.Ranger)
                    {
                        Manager_KJG.Audio.PlaySFX("hunter_Ranger_Attack");
                    }
                    else if (_hunterData._hunterJop == HunterJop.Sorcerer)
                    {
                        Manager_KJG.Audio.PlaySFX("hunter_Sorcerer_Attack");
                    }
                    else if (_hunterData._hunterJop == HunterJop.Berserker)
                    {
                        Manager_KJG.Audio.PlaySFX("hunter_Berserker_Attack");
                    }
                    else
                    {
                        Manager_KJG.Audio.PlaySFX("hunter_Paladin_Attack");
                    }
                }

                // 데미지 처리 (전투 스크립트 호출)
                if (_hunterData._attackRange <= 1.5f)
                {
                    if (_battle != null && _targetBattle != null)
                    {
                        _battle.GiveDamage(_targetBattle);
                    }
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

    #region 건물 상호작용 UI 호출용
    public void CommandBuilding(Transform buildingTarget, BuildingType_YHJ buildingType)
    {
        // ★ 2026-04-16 YHJ: UI에서 선택한 건물 타겟과 타입을 저장해 강제이동 후 BuildingInteraction으로 이어지게 함
        _buildingTarget = buildingTarget;
        _buildingType = buildingType;

        _targetMonster = null;  // 몬스터 무시
        _targetBattle = null;   // 전투 무시

        _targetPosition = buildingTarget.position;
        _isForcedMove = true;
    }
    #endregion

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
        // 헌터 위치기반 지역찾기 / 시작시 가만히 있는 상태 방지
        if (_targetBox == null)
        { 
            _targetBox = FindAreaPosition();
        }
        // 없으면 대기
        while (_targetBox == null)
        {
            _targetBox = FindAreaPosition();
            yield return null;
        }

        // 행동 루프 시작(1회)
        StartCoroutine(HunterActionCenterLoop());
    }
    #endregion

    #region UI
    public void OnClick()
    {
        if (_hunterData == null)
        {
            Debug.Log("헌터 정보가 없음");
            return;
        }
            
        HunterData_PJS.InfoHunter = _hunterData;

        UiManager.Instance.TargetHunter(true ,this); // Camera
        UiManager.Instance.hunterData = _hunterData; // UI
        Debug.Log("헌터 정보 카메라 지정");
    }
    #endregion
}
