using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DropTableManager_KJG
/// 
/// 역할:
/// - DropTableSO_KJG를 기반으로 몬스터 사망 시 드랍 처리
/// - minAmount ~ maxAmount 사이에서 랜덤하게 드랍
/// </summary>
public class DropTableManager_KJG : BaseManager_KJG<DropTableManager_KJG>
{
    [Header("드랍 테이블 데이터")]
    [SerializeField] private List<DropTableSO_KJG> dropTables;

    public void DropItems(BaseWorldObject_KJG deadObject)
    {
        if (dropTables == null || dropTables.Count == 0) return;

        Debug.Log($"[DropTableManager_KJG] {deadObject.displayName} 사망 → 드랍 처리");

        foreach (var table in dropTables)
        {
            var drops = table.GetRandomDrops();

            foreach (var drop in drops)
            {
                // minAmount ~ maxAmount 사이 랜덤 값 계산
                int actualAmount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                ProcessDrop(drop.itemType, actualAmount);
            }
        }
    }

    private void ProcessDrop(DropItemType itemType, int amount)
    {
        if (itemType == DropItemType.Gold)
        {
            Manager_KJG.Currency.AddGold(amount);
            Debug.Log($"[DropTableManager_KJG] 골드 {amount} 드랍");
        }
        else
        {
            Debug.Log($"[DropTableManager_KJG] {itemType} {amount}개 드랍");
            // 나중에 InventoryManager_KJG.AddItem(itemType, amount) 등으로 확장
        }
    }
}