using UnityEngine;

// ★ 스킬 기능 (UI 트리거용)
public class SkillInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    public bool CanInteract(IUnit_YHJ unit)
    {
        return !unit.IsDead;
    }

    public void Interact(IUnit_YHJ unit)
    {
        Debug.Log("스킬 UI 요청");

        EventBus_YHJ.RequestOpenSkillUI?.Invoke(unit);
    }
}