using UnityEngine;
using System.Collections;

public class HunterController_JS : MonoBehaviour
{
    private enum HunterState 
    { 
        Idle, Move, Attack
    }

    [SerializeField] private HunterState _currentState = HunterState.Idle;

    [Header("헌터 데이터 참조")]
    [SerializeField] private HunterData_JS _data;
    [SerializeField] private Animator _animator;

    [Header("이동 설정")]
    [SerializeField] private float _hunterMoveSpeed = 3.0f;
    [SerializeField] private float _idleTime = 1.0f;

    [Header("전투 설정")]
    [SerializeField] private float _hunterAttackSpeed = 1.0f;
    [SerializeField] private float _hunterAttackRange = 1.5f;

    [SerializeField] private BoxCollider2D _targetBox;   // 이동 영역

    private GameObject _targetMonster;  // 몬스터 감지
    private Vector2 _targetPosition;    // 이동 목적지 좌표
    private float _lookTargetX;         // 바라볼 타겟 X좌표
    private float _lastAttackTime;      // 마지막 공격 시점

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (HunterManager_JS.Instance != null)
        { 
            HunterManager_JS.Instance._activeHunters.Add(this);
        }
        UpdateLocation();
    }

    public void UpdateLocation()
    {
        // 기존 행동 중지
        StopAllCoroutines();

        if (HunterManager_JS.Instance != null)
        {
            HunterManager_JS.Instance._areaIndex = (int)_data._areaType;
            _targetBox = HunterManager_JS.Instance.GetAreaCollider();
        }

        RandomPos();
        
        if (gameObject.activeInHierarchy)
        { 
            // 새로운 행동 시작
            StartCoroutine(HunterActionCenterLoop());
        }
    }

    // 헌터 행동 중앙관리 코루틴
    IEnumerator HunterActionCenterLoop()
    {
        while (true)
        {
            // 타겟부터 감지
            FindTarget();

            // 타겟이 존재하고 범위내라면 공격
            if (_targetMonster != null && Vector2.Distance
                (
                    transform.position,
                    _targetMonster.transform.position
                ) <= _hunterAttackRange)
            {
                _currentState = HunterState.Attack;
                yield return StartCoroutine(HunterAttackLoop());
            }

            // 타겟이 존재하지 않으면 이동 후 _idleTime만큼 대기
            else
            { 
                _currentState= HunterState.Move;
                yield return StartCoroutine(HunterMoveLoop());

                _currentState= HunterState.Idle;
                _animator.SetBool("IsMoving", false);
                yield return new WaitForSeconds(_idleTime);
            }
        }
    }

    // 헌터 이동 코루틴
    IEnumerator HunterMoveLoop()
    {
        // 구역 확인 / 목적지 설정 없으면 중지
        if (_targetBox == null)
        {
            yield break;
        }

        // 마름모 좌표 추출 함수 호출
        RandomPos();
        // 방향 전환(목적지의 X좌표 대입 후 실행)
        _lookTargetX = _targetPosition.x;
        LookAt();

        _animator.SetBool("IsMoving", true);
        BoxCollider2D currentBox = _targetBox;
        
        // 목표 지점 이동(목표 도달까지) / 거리체크
        while (Vector2.Distance(transform.position, _targetPosition) > 0.1f)
        {
            if (_targetBox != currentBox)
            {
                Debug.Log("구역 변경 감지! 이동 중단 후 새 구역으로 전환");
                break;
            }
            // 이동중에도 타겟 감지
            FindTarget();
            // 타겟 발견시 이동 중단
            if (_targetMonster != null && Vector2.Distance
                (
                    transform.position,
                    _targetMonster.transform.position
                ) <= _hunterAttackRange)
            {
                break;
            }

            // 현재 위치 -> 목표위치 이동
            transform.position = Vector2.MoveTowards
                (
                    transform.position,
                    _targetPosition,
                    _hunterMoveSpeed * Time.deltaTime
                );
            yield return null;
        }
        _animator.SetBool("IsMoving", false);
    }

    // 헌터 공격 코루틴
    IEnumerator HunterAttackLoop()
    {
        // 타겟 확인 / 몬스터 없으면 중지
        if (_targetMonster == null)
        {
            yield break;
        }

        // 공격속도 체크
        if (Time.time >= _lastAttackTime + _hunterAttackSpeed)
        {
            // 방향 전환(몬스터의 X좌표 대입 후 실행)
            _lookTargetX = _targetMonster.transform.position.x;
            LookAt();

            _animator.SetTrigger("Attack");
            _lastAttackTime = Time.time;
        }
        yield return null;
    }

    // 태그를 이용한 몬스터 감지 함수
    private void FindTarget()
    {
        _targetMonster = GameObject.FindWithTag("Monster");
    }

    // 헌터 좌우 반전 함수
    private void LookAt()
    {
        // 오른쪽 이동시 캐릭터가 바라보는 방향
        if (_lookTargetX > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // 왼쪽 이동시 캐릭터가 바라보는 방향
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // 아이소매트릭 마름모 좌표 추출 / 내부 지점 계산 함수
    private void RandomPos()
    {
        // 지역의 경계 정보 가져옴
        Bounds areaBounds = _targetBox.bounds;

        while (true)
        {
            // 영역의 최소 ~ 최대 랜덤 좌표 추출
            float x = Random.Range(areaBounds.min.x, areaBounds.max.x);
            float y = Random.Range(areaBounds.min.y, areaBounds.max.y);
            _targetPosition = new Vector2(x, y);

            // 영역 내부로 들어오면 종료
            if (_targetBox.OverlapPoint(_targetPosition))
            {
                break;
            }
        }
    }
}
