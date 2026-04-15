using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [KJG 실무 최고 수준] ExpManager_KJG
///
/// 역할:
/// - 몬스터 사망 시 지역별 헌터에게 EXP 자동 분배 (원작과 동일)
/// - Monster는 EXP 로직을 전혀 알 필요 없음 → OnMonsterDied 이벤트만 발생
/// - 나중에 캐시아이템, 난이도 배율, 업적 연동 등 확장하기 매우 쉬움
/// </summary>
public class ExpManager_KJG : BaseManager_KJG<ExpManager_KJG>
{
    [Header("기본 EXP 설정")]
    [Tooltip("기본 몬스터 사망 EXP (난이도/배율은 DifficultyManager에서 가져옴)")]
    public int baseMonsterExp = 50;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [ExpManager_KJG] EXP 시스템 초기화 완료");
    }

    /// <summary>
    /// Monster_JBJ.Die()에서 호출됨
    /// Monster는 지역 정보를 몰라도 됩니다. ExpManager가 판단합니다.
    /// </summary>
    public void OnMonsterDied(Monster_JBJ monster)
    {
        if (monster == null) return;

        // 1. 몬스터의 현재 위치로 가장 가까운 AreaType 판단
        AreaType area = GetAreaFromPosition(monster.transform.position);

        // 2. 해당 지역의 헌터들에게 EXP 분배
        int finalExp = CalculateFinalExp(baseMonsterExp);

        if (Manager_KJG.Hunter != null)
        {
            Manager_KJG.Hunter.AddExpToHuntersInArea(finalExp, area);
            Debug.Log($"[ExpManager] {monster.displayName} 사망 → {area} 지역 헌터들에게 {finalExp} EXP 지급");
        }
        else
        {
            Debug.LogWarning("[ExpManager] HunterManager가 등록되지 않았습니다.");
        }
    }

    /// <summary>
    /// 몬스터 위치 → AreaType 판단 (실무에서 가장 안정적인 방법)
    /// </summary>
    private AreaType GetAreaFromPosition(Vector3 position)
    {
        if (Manager_KJG.Hunter == null) return AreaType.Village;

        // HunterManager가 가진 모든 Area Collider를 체크
        BoxCollider2D[] areas = Manager_KJG.Hunter.GetAllAreas(); // HunterManager에 이 메서드 추가 필요 (아래 참고)

        foreach (var areaCollider in areas)
        {
            if (areaCollider != null && areaCollider.bounds.Contains(position))
            {
                // AreaType은 Collider 이름이나 별도 enum 매핑으로 판단 (현재는 Village 기본)
                return AreaType.Village; // 필요하면 Collider Tag나 Layer로 확장 가능
            }
        }
        return AreaType.Village; // 기본값 (원작에서 대부분 Village)
    }

    /// <summary>
    /// 난이도, 캐시아이템, 배율 등을 적용한 최종 EXP 계산
    /// </summary>
    private int CalculateFinalExp(int baseExp)
    {
        float multiplier = 1f;

        if (Manager_KJG.Difficulty != null)
            multiplier *= Manager_KJG.Difficulty.GetCurrentExpMultiplier();

        if (Manager_KJG.Currency != null)
            multiplier *= Manager_KJG.Currency.expMultiplier;

        return Mathf.RoundToInt(baseExp * multiplier);
    }

    // ==================== 팀원에게 알려줄 내용 ====================
    // HunterManager_PJS.cs에 아래 메서드만 추가하면 됩니다:
    // public BoxCollider2D[] GetAllAreas() => _allArea;
}
