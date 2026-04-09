using UnityEngine;

/// <summary>
/// [KJG 실무 아키텍처] ExpManager_KJG
/// 
/// 역할:
/// - 몬스터 처치 시 같은 지역(Area)에 있는 헌터들에게 EXP 지급 (원작 고증)
/// - DropManager와 완전히 분리된 별도 시스템
/// </summary>
public class ExpManager_KJG : BaseManager_KJG<ExpManager_KJG>
{
    /// <summary>
    /// 같은 지역 헌터들에게 EXP 지급
    /// </summary>
    public void AddExpToHuntersInArea(int expAmount, AreaType areaType)
    {
        if (Manager_KJG.Hunter != null)
        {
            Manager_KJG.Hunter.AddExpToHuntersInArea(expAmount, areaType);
            Debug.Log($"[ExpManager_KJG] {areaType} 지역 헌터들에게 {expAmount} EXP 지급");
        }
        else
        {
            Debug.LogWarning("[ExpManager_KJG] HunterManager가 아직 초기화되지 않았습니다.");
        }
    }
}