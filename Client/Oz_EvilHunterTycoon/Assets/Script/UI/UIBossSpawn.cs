using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBossSpawn : MonoBehaviour
{
    public BossSpawner_JBJ bossspawner;

    public void SpawnBoss()
    {
        bossspawner.SpawnBossFromTown();
    }
}
