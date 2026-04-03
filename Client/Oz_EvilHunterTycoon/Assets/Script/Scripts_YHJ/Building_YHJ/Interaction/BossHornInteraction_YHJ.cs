using UnityEngine;

// ★ 기능: 보스 UI 열기 트리거
public class BossHornInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public bool CanInteract(IUnit_YHJ unit) => true;

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("보스 UI 요청");

        // ★ 기능: 보스 선택 UI 열기
        EventBus_YHJ.RequestOpenBossUI?.Invoke();
    }
}