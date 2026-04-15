using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DropManager_KJG
/// DropTableSO_KJG와 완전 연동 완료
/// </summary>
public class DropManager_KJG : BaseManager_KJG<DropManager_KJG>
{
    [Header("드랍 테이블")]
    [SerializeField] private List<DropTableSO_KJG> dropTables;

    [Header("드랍 프리팹")]
    [SerializeField] private GameObject dropItemPrefab;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [DropManager_KJG] 드랍 시스템 초기화 완료");
    }

    public void DropFromMonster(Monster_JBJ monster)
    {
        if (monster == null || dropTables == null || dropTables.Count == 0) return;

        Debug.Log($"[DropManager_KJG] {monster.displayName} 사망 → 드랍 시작");

        foreach (var table in dropTables)
        {
            var drops = table.GetDrops();
            foreach (var drop in drops)
            {
                CreateDropItem(drop, monster.transform.position);
            }
        }
    }

    private void CreateDropItem(DropTableSO_KJG.DropEntry drop, Vector3 position)
    {
        if (dropItemPrefab == null) return;

        Vector3 spawnPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0.2f, Random.Range(-0.5f, 0.5f));
        var itemGO = Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);
        var pickup = itemGO.GetComponent<DropItemPickup_KJG>();

        if (pickup != null)
        {
            pickup.SetDropData(drop.itemType, drop.amount);
        }
    }
}