using UnityEngine;

/// <summary>
/// DropManager_KJG
///
/// 몬스터 사망 시 재료 드랍을 중앙에서 처리
/// ExpManager와 완전히 분리되어 있어 유지보수가 매우 쉬움
/// </summary>
public class DropManager_KJG : BaseManager_KJG<DropManager_KJG>
{
    [Header("드랍 설정")]
    [Tooltip("드랍 확률 (0~1)")]
    public float dropChance = 0.7f;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [DropManager_KJG] 드랍 시스템 초기화 완료");
    }

    /// <summary>
    /// Monster_JBJ.Die()에서 호출됨
    /// </summary>
    public void DropFromMonster(Monster_JBJ monster)
    {
        if (monster == null || Random.value > dropChance) return;

        // TODO: 실제 드랍 테이블 SO를 사용해 재료 드랍 (DropTableSO_KJG 연동 예정)
        Debug.Log($"[DropManager] {monster.displayName} 사망 → 재료 드랍 발생!");

        // 나중에 DropTableSO_KJG와 연결할 부분
        // Manager_KJG.Building.ConsumeMaterial(...) 등으로 확장 가능
    }
}