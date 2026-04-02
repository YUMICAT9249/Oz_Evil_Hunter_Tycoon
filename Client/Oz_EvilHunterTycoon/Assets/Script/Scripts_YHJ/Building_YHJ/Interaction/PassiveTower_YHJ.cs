using UnityEngine;

// ★ 기능: 패시브 효과 제공
// - 건물 존재 시 지속 효과 적용
public class PassiveTower_YHJ : MonoBehaviour
{
    void Start()
    {
        Debug.Log("패시브 타워 활성화");

        // ★ 기능: 패시브 효과 적용 요청
        // EventBus_YHJ.RequestApplyPassive?.Invoke(...);
    }
}