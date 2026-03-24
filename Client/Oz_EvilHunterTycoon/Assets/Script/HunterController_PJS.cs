using UnityEngine;
using System.Collections;

public class HunterController_PJS : MonoBehaviour
{
    // 헌터 상태
    private enum HunterState
    {
        Idle, Move, Attack
    }

    [SerializeField] private HunterState _currentState = HunterState.Idle;
    [SerializeField] private AreaType _areaCheck; // 이전 지역 저장용

    [Header("헌터 데이터 참조")]
    [SerializeField] private HunterData_PJS _data;
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider2D _targetBox;   // 현재 이동 영역

    // 스텟 캐싱
    private float _hunterAttackSpeed;   // 공격 속도
    private float _hunterAttackRange;   // 공격 사거리
    private float _hunterDetectRange;   // 감지 거리
    private float _hunterMoveSpeed;     // 이동 속도

    // 내부 변수
    private Coroutine _mainRoutine;
    private GameObject _targetMonster;  // 현재 타겟
    private Vector2 _targetPosition;    // 이동 목적지
    private float _lookTargetX;
    private float _lastAttackTime;
    private float _idleTime = 1.0f;

    // 유저 명령 우선 처리용
    private bool _isForcedMove = false;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        // 데이터에서 스텟 가져오기
        if (_data != null)
        {
            _hunterAttackSpeed = _data.AttackSpeed;
            _hunterAttackRange = _data.AttackRange;
            _hunterDetectRange = _data.DetectRange;
            _hunterMoveSpeed = _data.MoveSpeed;
        }
    }

    void Start()
    {
        // 매니저에 등록
        if (HunterManager_PJS.Instance != null)
        {
            HunterManager_PJS.Instance._activeHunters.Add(this);
        }
        _areaCheck = _data._areaType;

        // 초기 위치 설정
        UpdateLocation();
        StartCoroutine(HunterActionCenterLoop());
    }

    void Update()
    {
        // 지역 변경 감지
        AreaCheck(); 
    }

    // 지역 변경 감지 (변경될 때만 실행)
    private void AreaCheck()
    {
        if (_data == null) return;

        if (_areaCheck != _data._areaType)
        {
            _areaCheck = _data._areaType;
            UpdateLocation();
        }
    }

    // 위치 갱신 (유저 명령 들어왔을 때 실행)
    public void UpdateLocation()
    {
        // 강제 이동 ON
        _isForcedMove = true;

        if (HunterManager_PJS.Instance != null)
        {
            // 새로운 지역 설정
            _targetBox = HunterManager_PJS.Instance.GetAreaCollider(_data._areaType);
        }

        if (_targetBox != null)
        {
            // 새로운 목적지 설정
            RandomPos();
        }
    }

    // 행동 중앙 제어
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
            if (_targetMonster == null || Vector2.Distance(transform.position, _targetMonster.transform.position) > _hunterDetectRange)
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
                float dis = Vector2.Distance(transform.position, _targetMonster.transform.position);

                // 공격 범위 안
                if (dis <= _hunterAttackRange)
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

    // 이동
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
            // 강제 이동 아닐 때만 몬스터 감지
            if (!_isForcedMove)
            {
                FindTarget();

                if (_targetMonster != null && Vector2.Distance(transform.position, _targetMonster.transform.position) <= _hunterDetectRange)
                {
                    yield break;
                }
            }

            // 지역 변경 감지
            if (_targetBox != currentBox)
            {
                yield break;
            }

            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _hunterMoveSpeed * Time.deltaTime);
            yield return null;
        }

        _animator.SetBool("IsMoving", false);
    }

    // 추격
    IEnumerator HunterFollowLoop()
    {
        _currentState = HunterState.Move;
        _animator.SetBool("IsMoving", true);

        while (_targetMonster != null &&
               Vector2.Distance(transform.position, _targetMonster.transform.position) > _hunterAttackRange)
        {
            _lookTargetX = _targetMonster.transform.position.x;
            LookAt();

            transform.position = Vector2.MoveTowards(transform.position, _targetMonster.transform.position, _hunterMoveSpeed * Time.deltaTime);
            yield return null;

            FindTarget();
        }

        _animator.SetBool("IsMoving", false);
    }

    // 공격
    IEnumerator HunterAttackLoop()
    {
        while (_targetMonster != null &&
               Vector2.Distance(transform.position, _targetMonster.transform.position) <= _hunterAttackRange)
        {
            if (Time.time >= _lastAttackTime + _hunterAttackSpeed)
            {
                _lookTargetX = _targetMonster.transform.position.x;
                LookAt();

                _animator.SetTrigger("Attack");
                _lastAttackTime = Time.time;

                yield return new WaitForSeconds(_hunterAttackSpeed);
            }
            else
            {
                yield return null;
            }

            FindTarget();
        }
    }

    // 몬스터 찾기
    private void FindTarget()
    {
        GameObject monster = GameObject.FindWithTag("Monster");

        if (monster == null)
        {
            _targetMonster = null;
            return;
        }

        if (!_targetBox.OverlapPoint(monster.transform.position))
        {
            _targetMonster = null;
            return;
        }
        _targetMonster = monster;
    }

    // 방향 전환
    private void LookAt()
    {
        if (_lookTargetX > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    // 랜덤 위치 생성
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

    public void HunterDie()
    { 
        StopAllCoroutines();
        _animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
    }

    public void ReturnVillage()
    {
        _data._areaType = AreaType.Village;
        BoxCollider2D villageBox = HunterManager_PJS.Instance.GetAreaCollider(AreaType.Village);
        if (villageBox != null)
        {
            transform.position = villageBox.bounds.center;
        }

        // 알파 값 복구
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        { 
            Color color = sr.color;
            color.a = 1f;
            sr.color = color;
        }
        // HP 최대치 + 부활처리
        _data.HP = _data._maxHP;
        GetComponent<Collider2D>().enabled = true;
        UpdateLocation();
    }
}