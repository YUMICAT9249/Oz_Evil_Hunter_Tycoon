using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 난이도 시스템 (추후 활성화)
/*
public enum Difficulty
{
    Easy,
    Normal,
    Hard
}
*/

public class Boss_JBJ : Monster_JBJ
{
    [Header("하수인 설정")]
    public GameObject minionPrefab;

    public float summonInterval = 30f;
    public int summonCount = 3;
    public int maxMinions = 6;

    private float summonTimer;
    private int currentMinionCount;

    private BossSpawner_JBJ bossSpawner;

    public void InitBoss(BossSpawner_JBJ spawner)
    {
        this.bossSpawner = spawner;
    }

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        summonTimer += Time.deltaTime;

        if (summonTimer >= summonInterval)
        {
            TrySummonMinions();
            summonTimer = 0f;
        }
    }

    void TrySummonMinions()
    {
        if (currentMinionCount >= maxMinions)
        {
            Debug.Log("최대 하수인 수에 도달했습니다.");
            return;
        }

        int spawnAmount = Mathf.Min(summonCount, maxMinions - currentMinionCount);

        for (int i = 0; i < spawnAmount; i++)
        {
            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(1.5f, 2.5f);

            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);

            GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);

            Monster_JBJ m = minion.GetComponent<Monster_JBJ>();
            m.InitMinion(this);

            currentMinionCount++;
        }

        Debug.Log($"{spawnAmount}명의 하수인이 소환되었습니다. 현재 하수인 수: {currentMinionCount}");
    }

    public void OnMinionDead()
    {
        currentMinionCount--;

        if (currentMinionCount < 0) 
            currentMinionCount = 0;
    }

    protected override void Die()
    {
        if (isDead) return;

        if (bossSpawner != null)
        {
            bossSpawner.OnBossDead();
        }

        else
        {
            Debug.LogWarning("BossSpawner가 연결되지 않음.");
        }

        base.Die();
    }
}
