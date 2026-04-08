using UnityEngine;

/// <summary>
/// ExpManager_KJG
/// 
/// 역할:
/// - 몬스터를 처치하면 **같은 지역(Area)에 있는 헌터들**에게 EXP를 지급합니다.
/// - Drop(드랍)과 완전히 분리된 별도 시스템입니다. (원작과 동일)
/// - Manager_KJG.Hunter를 통해 안전하게 접근합니다.
/// </summary>
public class ExpManager_KJG : BaseManager_KJG<ExpManager_KJG>
{
    /// <summary>
    /// 같은 지역 헌터들에게 EXP 지급 (원작에 가장 가까운 방식)
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