using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DropTableSO_KJG
/// 
/// 역할:
/// - 몬스터가 죽을 때 바닥에 떨어질 드랍 아이템을 정의하는 ScriptableObject
/// - 원작처럼 "단순 확률 + 고정 수량" 방식으로 제작
/// - EXP는 헌터 쪽에서 별도로 처리 (드랍과 분리)
/// 
/// 사용 방법:
/// 1. Project 창에서 오른쪽 클릭 → Create → KJG → Drop Table
/// 2. Inspector에서 drops 리스트에 아이템 추가
/// 3. DropManager_KJG.cs의 dropTables 리스트에 연결
/// </summary>
[CreateAssetMenu(menuName = "KJG/Drop Table", fileName = "New DropTable")]
public class DropTableSO_KJG : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        [Header("드랍될 아이템 종류")]
        public DropItemType itemType;

        [Header("드랍 수량")]
        public int amount = 10;

        [Header("드랍 확률 (%)")]
        [Range(0f, 100f)]
        public float chance = 80f;
    }

    [Header("이 몬스터가 드랍할 아이템 목록")]
    public List<DropEntry> drops = new List<DropEntry>();

    /// <summary>
    /// 몬스터 사망 시 호출 → 드랍할 아이템 리스트 반환
    /// </summary>
    public List<DropEntry> GetDrops()
    {
        List<DropEntry> result = new List<DropEntry>();

        foreach (var entry in drops)
        {
            if (Random.value * 100f <= entry.chance)
            {
                result.Add(entry);
            }
        }

        return result;
    }
}

/// <summary>
/// 드랍 아이템 종류 (원작에서 나오는 주요 아이템들)
/// </summary>
public enum DropItemType
{
    Gold,           // 골드
    Material,       // 일반 재료
    RareMaterial,   // 희귀 재료
    Essence         // 에센스 (고급 재료)
}