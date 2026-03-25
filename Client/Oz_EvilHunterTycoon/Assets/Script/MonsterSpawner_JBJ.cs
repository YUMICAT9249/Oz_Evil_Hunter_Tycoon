using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner_JBJ : MonoBehaviour
{
    public GameObject[] normalMonsters;
    public GameObject uniqueMonster;
    public UnitData_JBJ_PJS monsterData;

    // 난이도 시스템 (추후 활성화)
    // public Difficulty difficulty;

    public float spawnInterval = 5f;
    public int maxMonsterCount = 13;

    private float timer;
    private int currentCount;
    private int killCount;

    private bool uniqueSpawned = false;

    /*
    void Start()
    {
        // 난이도 별 설정 (추후 활성화)
        switch (difficulty)
        {
            case Difficulty.Easy:
                maxMonsterCount = 7;
                break;

            case Difficulty.Normal:
                maxMonsterCount = 9;
                break;

            case Difficulty.Hard:
                maxMonsterCount = 13;
                break;
        }
    }
    */


    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentCount < maxMonsterCount)
        {
            SpawnNormalMonster();
            timer = 0f;
        }
    }

    void SpawnNormalMonster()
    {
        Vector3 spawnPos = transform.position + new Vector3
            (
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f), 
                0
            );

        GameObject prefab = normalMonsters[Random.Range(0, normalMonsters.Length)];

        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity);

        Monster_JBJ m = monster.GetComponent<Monster_JBJ>();
        m.Init(this, MonsterType.Normal);

        currentCount++;
    }

    void SpawnUniqueMonster()
    {
        Vector3 spawnPos = transform.position;
            
        GameObject monster = Instantiate(uniqueMonster, spawnPos, Quaternion.identity);

        Monster_JBJ m = monster.GetComponent<Monster_JBJ>();
        m.Init(this, MonsterType.Unique);
    }

    public void OnMonsterDead(MonsterType type)
    {
        if (type == MonsterType.Normal)
        {
            currentCount--;
            killCount++;

            if (killCount >= 30 && !uniqueSpawned)
            {
                SpawnUniqueMonster();
                uniqueSpawned = true;
            }
        }
    }
}
