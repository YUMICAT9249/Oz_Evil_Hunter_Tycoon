using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner_JBJ : MonoBehaviour
{
    public GameObject monsterPrefab;
    public UnitData_JBJ_PJS monsterData;

    public float spawnInterval = 5f;
    public int maxMonsterCount = 13;

    private float timer;
    private int currentCount;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentCount < maxMonsterCount)
        {
            SpawnMonster();
            timer = 0f;
        }
    }

    void SpawnMonster()
    {
        Vector3 spawnPos = transform.position + new Vector3
            (
                Random.Range(-2f, 2f),
                Random.Range(-2f, 2f), 
                0
            );

        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

        Monster_JBJ m = monster.GetComponent<Monster_JBJ>();

        currentCount++;
    }
}
