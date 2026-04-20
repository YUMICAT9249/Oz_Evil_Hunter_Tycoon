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

public class BossSpawner_JBJ : MonoBehaviour
{
    public GameObject bossPrefab;

    private bool bossAlive = false;

    public void SpawnBossFromTown()
    {
        if (bossAlive)
        {
            Debug.Log("보스는 이미 존재함.");
            return;
        }

        SpawnBoss();
    }

    void SpawnBoss()
    {
        //KSH
        UiManager.Instance.BossUI();

        Vector3 spawnPos = transform.position;

        GameObject boss = Instantiate(bossPrefab, transform.position, Quaternion.identity);

        Boss_JBJ bossScript = boss.GetComponent<Boss_JBJ>();
        bossScript.InitBoss(this);

        bossAlive = true;

        Debug.Log("보스가 소환되었습니다!");
    }

    public void OnBossDead()
    {
        bossAlive = false;
        Debug.Log("보스가 처치되었습니다!");
    }
}
