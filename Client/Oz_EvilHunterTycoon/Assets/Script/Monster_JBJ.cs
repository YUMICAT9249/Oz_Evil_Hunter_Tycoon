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
        base.Start();
        currentHP = data.maxHp;
        renderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        battle = GetComponent<Battle_JBJ_PJS>();

        FindHunter();
        SetRandomDirection();
        SetMoveState();
    }

    protected virtual void Update()
    {
        if (isDead) return;
        // 기존 Update 로직 (원본 그대로 유지)
    }

    private void FindHunter()
    {
        Hunter = GameObject.FindWithTag("Hunter")?.transform;
    }

    private void SetRandomDirection()
    {
        moveDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
        lastMoveDir = moveDirection;
    }

    private void SetMoveState()
    {
        stateTimer = 0f;
        moveDuration = Random.Range(2f, 5f);
        idleDuration = Random.Range(1f, 3f);
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        // ★ 실무 최고 수준: Monster는 EXP/드랍 로직을 직접 하지 않음
        if (Manager_KJG.Exp != null)
            Manager_KJG.Exp.OnMonsterDied(this);

        if (Manager_KJG.Drop != null)
            Manager_KJG.Drop.DropFromMonster(this);

        if (Manager_KJG.Audio != null)
            Manager_KJG.Audio.PlaySFX("monster_death");

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

        float angle = Random.Range(-35f, 35f);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}