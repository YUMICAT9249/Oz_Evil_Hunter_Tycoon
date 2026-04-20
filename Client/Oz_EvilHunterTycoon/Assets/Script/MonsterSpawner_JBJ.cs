using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner_JBJ : MonoBehaviour
{
    public GameObject[] normalMonsters;
    public GameObject uniqueMonster;
    public UnitData_JBJ_PJS monsterData;

    public float spawnInterval = 5f;
    public int maxMonsterCount = 7;

    public Vector3 spawnAreaMin = new Vector3(-2, -2, 0);
    public Vector3 spawnAreaMax = new Vector3(2, 2, 0);

    private float timer;
    private int currentCount;
    private int killCount;

    private bool uniqueSpawned = false;

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

        m.minBounds = transform.position + spawnAreaMin;
        m.maxBounds = transform.position + spawnAreaMax;

        currentCount++;
    }

    void SpawnUniqueMonster()
    {
        Vector3 spawnPos = transform.position;
            
        GameObject monster = Instantiate(uniqueMonster, spawnPos, Quaternion.identity);

        Monster_JBJ m = monster.GetComponent<Monster_JBJ>();
        m.Init(this, MonsterType.Unique);

        m.minBounds = transform.position + spawnAreaMin;
        m.maxBounds = transform.position + spawnAreaMax;
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
