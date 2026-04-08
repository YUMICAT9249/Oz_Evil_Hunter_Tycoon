using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [KJG 실무 아키텍처] DropManager_KJG - 원작 스타일 드랍 매니저
/// 
/// 몬스터 사망 시 바닥에 드랍 아이템 생성
/// EXP는 HunterManager_PJS 쪽에서 별도로 처리
/// </summary>
public class DropManager_KJG : BaseManager_KJG<DropManager_KJG>
{
    [Header("드랍 테이블")]
    [SerializeField] private List<DropTableSO_KJG> dropTables;

    [Header("드랍 프리팹")]
    [SerializeField] private GameObject dropItemPrefab;   // 바닥에 떨어질 드랍 아이템 Prefab

    /// <summary>
    /// 몬스터가 죽을 때 호출 (Monster_JBJ.cs에서 호출)
    /// </summary>
    public void DropFromMonster(BaseWorldObject_KJG deadMonster)
    {
        if (dropTables == null || dropTables.Count == 0) return;

        Debug.Log($"[DropManager_KJG] {deadMonster.displayName} 사망 → 드랍 시작");

        foreach (var table in dropTables)
        {
            var drops = table.GetDrops();

            foreach (var drop in drops)
            {
                CreateDropItem(drop, deadMonster.transform.position);
            }
        }
    }

    private void CreateDropItem(DropTableSO_KJG.DropEntry drop, Vector3 position)
    {
        if (dropItemPrefab == null) return;

        Vector3 spawnPos = position + new Vector3(Random.Range(-0.5f, 0.5f), 0.2f, Random.Range(-0.5f, 0.5f));

        var itemGO = Instantiate(dropItemPrefab, spawnPos, Quaternion.identity);

        // 드랍 아이템에 정보 전달 (나중에 DropItemPickup_KJG 스크립트에서 사용)
        var pickup = itemGO.GetComponent<DropItemPickup_KJG>();
        if (pickup != null)
        {
            pickup.SetDropData(drop.itemType, drop.amount);
        }
    }
}