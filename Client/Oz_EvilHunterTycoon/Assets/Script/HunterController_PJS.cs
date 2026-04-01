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

    // [2] 참조 (타 스크립트 캐싱 제거 / 필요시 TryGetComponent 사용)
    [Header("이동 영역")]
    [SerializeField] private BoxCollider2D _targetBox;   // 현재 이동 영역

    // [3] 이동 타겟 관련
    private GameObject _targetMonster;  // 현재 타겟
    private Vector2 _targetPosition;    // 이동 목적지
    private float _lookTargetX;

    // [4] 내부 상태값
    private float _lastAttackTime;
    private float _idleTime = 1.0f;
    private bool _isForcedMove = false;

    // [5] 초기화
    void Start()
    {
        // 시작 시 현재 구역을 가져옴
        if (TryGetComponent(out HunterData_PJS hunterData))
        {
            // 기본 지역 저장
            _areaCheck = hunterData._areaType;
        }
        // 행동 루프 시작(1회)
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
        if (TryGetComponent(out HunterData_PJS hunterData))
        {
            if (_areaCheck != hunterData._areaType)
            {
                _areaCheck = hunterData._areaType;
                // 구역이 변경되면 "외부 매니저"에서 SetArea 호출
            }
        }
    }

    // [7] 위치 갱신 (매니저가 소환/이동 시 직접 호출)
    public void SetArea(BoxCollider2D newArea)
    {
        _targetBox = newArea;
        // 강제 이동 ON
        _isForcedMove = true;

        if (_targetBox != null)
        {
            // 새로운 목적지 설정
            RandomPos();
        }
    }

    // [8] 행동 중앙 제어(메인)
    IEnumerator HunterActionCenterLoop()
    {
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
                GetComponent<Animator>().SetBool("IsMoving", false);
                yield return new WaitForSeconds(_idleTime);
            }
            // 4. 타겟 있음
            else
            {
                if (TryGetComponent(out HunterData_PJS hunterData))
                {
                    float distance = Vector2.Distance(transform.position, _targetMonster.transform.position);

                    // 공격 범위 안
                    if (distance <= hunterData._unitData.attackRange)
                    {
                        _currentState = HunterState.Attack;
                        GetComponent<Animator>().SetBool("IsMoving", false);
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

        if (!_isForcedMove)
        {
            RandomPos();
        }

        _lookTargetX = _targetPosition.x;
        LookAt();
        GetComponent<Animator>().SetBool("IsMoving", true);

        while (Vector2.Distance(transform.position, _targetPosition) > 0.1f)
        {
            // 주변에 몬스터가 있는지 확인
            FindTarget();

            // 몬스터를 찾았다면
            if (_targetMonster != null)
            {
                // 이동루프 탈출 -> HunterActionCenterLoop()로 복귀
                GetComponent<Animator>().SetBool("IsMoving", false);
                yield break;
            }

            // 이동속도 적용
            if (TryGetComponent(out HunterData_PJS hunterData))
            {
                transform.position = Vector2.MoveTowards
                    (
                        transform.position, 
                        _targetPosition, 
                        hunterData.GetMoveSpeed() * Time.deltaTime
                    );
            }
            yield return null;
        }
        GetComponent<Animator>().SetBool("IsMoving", false);
    }

    // [10] 추격
    IEnumerator HunterFollowLoop()
    {
        _currentState = HunterState.Move;
        GetComponent<Animator>().SetBool("IsMoving", true);

        while (_targetMonster != null)
        {
            if (TryGetComponent(out HunterData_PJS hunterData))
            {
                float distance = Vector2.Distance(transform.position, _targetMonster.transform.position);

                if (distance <= hunterData._unitData.attackRange)
                {
                    GetComponent<Animator>().SetBool("IsMoving", false);
                    yield break;
                }

                _lookTargetX = _targetMonster.transform.position.x;
                LookAt();

                transform.position = Vector2.MoveTowards
                    (
                        transform.position, 
                        _targetMonster.transform.position, 
                        hunterData.GetMoveSpeed() * Time.deltaTime
                    );
            }
            yield return null;
        }
        GetComponent<Animator>().SetBool("IsMoving", false);
    }

    // [11] 공격
    IEnumerator HunterAttackLoop()
    {
        while (_targetMonster != null)
        {
            if (_targetMonster == null) yield break;

            if (TryGetComponent(out HunterData_PJS hunterData))
            {
                float cooldown = hunterData.GetAttackCooldown();

                if (Time.time >= _lastAttackTime + cooldown)
                {
                    _lookTargetX = _targetMonster.transform.position.x;
                    LookAt();
                    Animator animator = GetComponent<Animator>();
                    // 애니메이션 공격 쿨다운 조절(공격속도 조절)
                    animator.speed = hunterData._unitData.attackCooldown / cooldown;
                    animator.SetTrigger("Attack");

                    // 데미지 처리 (전투 스크립트 호출)
                    if (TryGetComponent(out Battle_JBJ_PJS battle))
                    {
                        battle.GiveDamage(_targetMonster);
                    }
                    _lastAttackTime = Time.time;

                    yield return new WaitForSeconds(cooldown);
                    // 공격 후 애니메이션 속도 복구
                    animator.speed = 1.0f;
                }
                else
                {
                    yield return null;
                }
            }
            FindTarget();
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
            }      
            // 범위 안이면 타겟 유지
            else return;
        }
        // 2. 새로운 몬스터 탐지
        GameObject monster = GameObject.FindWithTag("Monster");
        if (monster == null) return;
        // 구역 체크
        if (!_targetBox.OverlapPoint(monster.transform.position)) return;
        if (TryGetComponent(out HunterData_PJS hunterData))
        {
            // 거리 체크
            float distance = Vector2.Distance(transform.position, monster.transform.position);
            if (distance <= hunterData._unitData.detectRange)
            {
                _targetMonster = monster;
            }
        }
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

    // [15] 헌터 사망 처리 함수
    public void HunterDie()
    {
        StopAllCoroutines();
        GetComponent<Animator>().SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;

        // 사망 이벤트
        HunterData_PJS.OnHunterDie?.Invoke();
    }

    // [16] 마을 귀환 함수
    public void ReturnVillage(BoxCollider2D villageBox)
    {
        if (TryGetComponent(out HunterData_PJS hunterData))
        {
            hunterData._areaType = AreaType.Village;
            _areaCheck = AreaType.Village;

            if (villageBox != null)
            {
                transform.position = villageBox.bounds.center;
                _targetBox = villageBox; // 구역 갱신
            }

            // 반투명 알파 값 복구
            if (TryGetComponent(out SpriteRenderer sr))
            {
                Color color = sr.color;
                color.a = 1f;
                sr.color = color;
            }

            // HP 최대치 + 부활처리
            hunterData._currentHP = hunterData.GetMaxHP();
            GetComponent<Collider2D>().enabled = true;
            SetArea(_targetBox); // 위치갱신 로직 실행
        }
    }
}