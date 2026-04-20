using System.Collections;
using UnityEngine;

public enum MonsterType
{
    Normal,
    Unique,
    Minion
}

public class Monster_JBJ : BaseWorldObject_KJG
{
    [Header("몬스터 기본 정보")]
    public string monsterName;
    public UnitData_JBJ_PJS data;
    public float currentHP;
    public float lastAttackTime;
    public Transform Hunter;
    public Vector3 moveDirection;
    public Vector3 minBounds;
    public Vector3 maxBounds;

    Vector3 lastMoveDir;
    SpriteRenderer[] renderers;
    bool isIdle = false;
    protected bool isDead = false;
    float stateTimer;
    float moveDuration;
    float idleDuration;
    int facingDir = -1;
    Animator animator;
    MonsterSpawner_JBJ spawner;
    MonsterType type;
    Battle_JBJ_PJS battle;
    Boss_JBJ boss;

    // ================================================
    // ★ KJG 추가: 이 몬스터 전용 드랍 테이블
    // 한 마리당 하나의 테이블만 지정하면 됩니다.
    // (이전처럼 모든 테이블이 터지는 문제 해결)
    [Header("드랍 테이블 (이 몬스터만 드랍)")]
    [SerializeField] private DropTableSO_KJG monsterDropTable;
    // ================================================

    protected override void Awake()
    {
        base.Awake();
        displayName = monsterName;
    }

    public void Init(MonsterSpawner_JBJ spawner, MonsterType type)
    {
        this.spawner = spawner;
        this.type = type;
    }

    public void InitMinion(Boss_JBJ boss)
    {
        this.boss = boss;
        this.spawner = null;
        this.type = MonsterType.Minion;
    }

    protected override void Start()
    {
        currentHP = data.maxHp;
        renderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponentInParent<Animator>();
        battle = GetComponent<Battle_JBJ_PJS>();
        FindHunter();
        SetRandomDirection();
        SetMoveState();
    }

    protected virtual void Update()
    {
        if (isDead) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Kill();
            return;
        }

        stateTimer -= Time.deltaTime;
        if (moveDirection != Vector3.zero)
        {
            lastMoveDir = moveDirection;
        }
        if (stateTimer <= 0)
        {
            if (isIdle)
                SetMoveState();
            else
                SetIdleState();
        }
        bool isMovingNow = false;
        if (Hunter == null)
        {
            FindHunter();
            if (!isIdle)
            {
                Move();
                isMovingNow = true;
            }
            Flip();
            animator.SetBool("isMoving", isMovingNow);
            return;
        }
        float distance = Vector3.Distance(transform.position, Hunter.position);
        if (distance <= data.detectRange)
        {
            isIdle = false;
            if (distance <= data.attackRange)
            {
                Attack();
                isMovingNow = false;
            }
            else
            {
                ChaseHunter();
                isMovingNow = true;
            }
        }
        else
        {
            if (!isIdle)
            {
                Move();
                isMovingNow = true;
            }
        }
        Flip();
        animator.SetBool("isMoving", isMovingNow);
    }

    private void FindHunter()
    {
        Hunter = GameObject.FindWithTag("Hunter")?.transform;
    }

    private void Move()
    {
        Vector3 pos = transform.position;
        pos += moveDirection * data.moveSpeed * Time.deltaTime;
        bool bounced = false;
        if (pos.x < minBounds.x || pos.x > maxBounds.x)
        {
            moveDirection.x *= -1;
            bounced = true;
        }
        if (pos.y < minBounds.y || pos.y > maxBounds.y)
        {
            moveDirection.y *= -1;
            bounced = true;
        }
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        transform.position = pos;
        if (bounced)
        {
            moveDirection += new Vector3
                (
                    Random.Range(-0.3f, 0.3f),
                    Random.Range(-0.3f, 0.3f),
                    0
                );
        }
        moveDirection.Normalize();
    }

    private void SetRandomDirection()
    {
        moveDirection = new Vector3
            (
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0
            ).normalized;
    }

    private void SetMoveState()
    {
        isIdle = false;
        moveDuration = Random.Range(2f, 4f);
        stateTimer = moveDuration;
        SetRandomDirection();
    }

    private void SetIdleState()
    {
        isIdle = true;
        idleDuration = Random.Range(1f, 2.5f);
        stateTimer = idleDuration;
    }

    private void SetFacing(int dir)
    {
        if (dir == facingDir) return;
        facingDir = dir;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * -facingDir;
        transform.localScale = scale;
    }

    private void ChaseHunter()
    {
        Vector3 dir = (Hunter.position - transform.position).normalized;
        transform.position += dir * data.moveSpeed * Time.deltaTime;
        moveDirection = dir;
        if (dir.x > 0) SetFacing(1);
        else if (dir.x < 0) SetFacing(-1);
    }

    private void Flip()
    {
        if (Mathf.Abs(moveDirection.x) < 0.01f) return;
        int newDir = moveDirection.x > 0 ? 1 : -1;
        if (newDir != facingDir)
        {
            facingDir = newDir;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -facingDir;
            transform.localScale = scale;
        }
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime < data.attackCooldown) return;
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        PlayAttackSound();
        Debug.Log("Monster attacks (Hunter)");
    }

    public void PlayAttackSound()
    {
        Manager_KJG.Audio.PlaySFX("Monster Attack");
    }

    public void ApplyDamageToHunter()
    {
        if (Hunter == null || battle == null) return;
        Battle_JBJ_PJS targetBattle = Hunter.GetComponent<Battle_JBJ_PJS>();
        battle.GiveDamage(targetBattle);
    }

    public void MonsterTakeDamage(float damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
        OnHealthChanged(currentHP, maxHp);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // ★ 실무 최고 수준: Monster는 EXP/드랍 로직을 직접 하지 않음
        if (Manager_KJG.Exp != null)
            Manager_KJG.Exp.OnMonsterDied(this);

        // ★ KJG 수정: 이 몬스터 전용 테이블만 드랍 (모든 테이블이 터지는 문제 해결)
        if (Manager_KJG.Drop != null && monsterDropTable != null)
        {
            Manager_KJG.Drop.DropFromTable(monsterDropTable, transform.position);
        }
        else if (monsterDropTable == null)
        {
            Debug.LogWarning($"[Monster_JBJ] {displayName}에 DropTable이 지정되지 않았습니다.");
        }

        if (Manager_KJG.Audio != null)
            Manager_KJG.Audio.PlaySFX("Monster Die");

        if (Manager_KJG.Achievement != null)
            Manager_KJG.Achievement.OnMonsterKilled();

        if (spawner != null)
            spawner.OnMonsterDead(type);
        if (boss != null)
            boss.OnMinionDead();

        StartCoroutine(DieRoutine());
    }

    public void Kill()
    {
        if (isDead) return;
        Die();
    }

    IEnumerator DieRoutine()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (lastMoveDir == Vector3.zero)
            lastMoveDir = new Vector3(facingDir, 0, 0);
        Vector3 hitDir = -lastMoveDir;
        transform.position += new Vector3(hitDir.x * 0.1f, -0.05f, 0);
        float angle = (hitDir.x >= 0) ? 90f : -90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}