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
        if (monster == null)
        {
            Debug.LogWarning("[DropManager_KJG] monster is null");
            return;
        }

        if (dropTables == null || dropTables.Count == 0)
        {
            Debug.LogWarning("[DropManager_KJG] Drop Tables 리스트가 비어있습니다!");
            return;
        }

        if (dropItemPrefab == null)
        {
            Debug.LogError("[DropManager_KJG] Drop Item Prefab이 할당되지 않았습니다!");
            return;
        }

        Debug.Log($"[DropManager_KJG] {monster.displayName} 사망 → 드랍 시작 (테이블 {dropTables.Count}개)");

        foreach (var table in dropTables)
        {
            var drops = table.GetDrops();
            Debug.Log($"[DropManager_KJG] 테이블에서 {drops.Count}개 드롭 결정됨");

            foreach (var drop in drops)
            {
                CreateDropItem(drop, monster.transform.position);
            }
        }
    }

    private void CreateDropItem(DropTableSO_KJG.DropEntry drop, Vector3 position)
    {
        Vector3 spawnPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0.2f, Random.Range(-0.5f, 0.5f));
        var itemGO = Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);
        var pickup = itemGO.GetComponent<DropItemPickup_KJG>();

        if (pickup != null)
        {
            pickup.SetDropData(drop.itemType, drop.amount);
            Debug.Log($"[DropManager_KJG] 드랍 아이템 생성 성공: {drop.itemType} x {drop.amount}");
        }
        else
        {
            Debug.LogError("[DropManager_KJG] DropItemPickup_KJG 컴포넌트가 Prefab에 없습니다!");
        }
    }
}