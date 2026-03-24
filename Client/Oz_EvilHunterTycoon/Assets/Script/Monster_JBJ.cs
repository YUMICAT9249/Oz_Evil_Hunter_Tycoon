using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_JBJ : MonoBehaviour
{
    public UnitData_JBJ_PJS data;

    public float currentHP;
    public Transform Hunter;

    public float lastAttackTime;
    public Vector3 moveDirection;

    public Vector3 minBounds;
    public Vector3 maxBounds;

    SpriteRenderer[] renderers;

    bool isIdle = false;

    float stateTimer;
    float moveDuration;
    float idleDuration;

    int facingDir = -1; // 1: 오른쪽, -1: 왼쪽

    Animator animator;

    void Start()
    {
        currentHP = data.maxHp;

        renderers = GetComponentsInChildren<SpriteRenderer>();

        animator = GetComponent<Animator>();

        FindHunter();

        SetRandomDirection();

        SetMoveState();
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0)
        {
            if (isIdle)
                SetMoveState();

            else
                SetIdleState();
        }

        bool isMovingNow = false;

        // 우선은 탐색, 이동만
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

    void FindHunter()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Hunter");

        if (obj != null)
        {
            Hunter = obj.transform;
        }
    }

    void Move()
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

    void SetRandomDirection()
    {
        moveDirection = new Vector3
            (
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f), 
                0
            ).normalized;
    }

    void SetMoveState()
    {
        isIdle = false;
        moveDuration = Random.Range(2f, 4f);
        stateTimer = moveDuration;

        SetRandomDirection();
    }

    void SetIdleState()
    {
        isIdle = true;
        idleDuration = Random.Range(1f, 2.5f);
        stateTimer = idleDuration;
    }

    void SetFacing(int dir)
    {
        if (dir == facingDir) return;

        facingDir = dir;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDir;
        transform.localScale = scale;
    }

    void ChaseHunter()
    {
        Vector3 dir = (Hunter.position - transform.position).normalized;

        transform.position += dir * data.moveSpeed * Time.deltaTime;

        moveDirection = dir;

        if (dir.x > 0) SetFacing(1);
        else if (dir.x < 0) SetFacing(-1);
    }

    void Flip()
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

    void Attack()
    {
        if (Time.time - lastAttackTime < data.attackCooldown) return;
        
        lastAttackTime = Time.time;

        Debug.Log("Monster attacks (Hunter)");

        // 헌터 스크립트 만들어지면 활성화
        /*
        var hunterComponent = Hunter.GetComponent<Hunter>(); 
        if (hunterComponent != null)
        {
            hunterComponent.TakeDamage(data.attackDamage);
        }
        */
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
