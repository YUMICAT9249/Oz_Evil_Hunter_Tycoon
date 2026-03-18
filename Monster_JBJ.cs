using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_JBJ : MonoBehaviour
{
    public MonsterData_JBJ data;

    public float currentHP;
    public Transform Hunter;

    public float lastAttackTime;
    public Vector3 moveDirection;

    void Start()
    {
        currentHP = data.maxHp;

        FindHunter();

        SetRandomDirection();
    }

    void Update()
    {
        // 우선은 탐색, 이동만
        if (Hunter == null)
        {
            FindHunter();
            Move();
            return;
        }

        float distance = Vector3.Distance(transform.position, Hunter.position);

        if (distance <= data.detectRange)
        {
            if (distance <= data.attackRange)
            {
                Attack();
            }

            else
            {
                ChaseHunter();
            }
        }

        else
        {
            Move();
        }
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
        transform.position += moveDirection * data.moveSpeed * Time.deltaTime;
    }

    void SetRandomDirection()
    {
        moveDirection = new Vector3
            (
                Random.Range(-1f, 1f), 
                0, 
                Random.Range(-1f, 1f)
            ).normalized;
    }

    void ChaseHunter()
    {
        Vector3 dir = (Hunter.position - transform.position).normalized;

        transform.position += dir * data.moveSpeed * Time.deltaTime;
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
