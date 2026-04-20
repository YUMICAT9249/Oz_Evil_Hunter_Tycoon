using UnityEngine;

public class ItemEffectProcessor_YHJ : MonoBehaviour
{
    public bool ApplyItem(string itemID, IUnit_YHJ unit)
    {
        ItemData_YHJ data = ItemDatabase_YHJ.Instance.Get(itemID);

        if (data == null)
        {
            Debug.LogWarning($"[Effect] 데이터 없음: {itemID}");
            return false;
        }

        switch (data.effectType)
        {
            case ItemEffectType_YHJ.HealHP:
                unit.Heal(data.value);
                return true;

            case ItemEffectType_YHJ.IncreaseDefence:
            case ItemEffectType_YHJ.IncreaseMoveSpeed:
            case ItemEffectType_YHJ.IncreaseDropAmount:
            case ItemEffectType_YHJ.IncreaseDamage:
                // ★ YHJ: 버프형 포션 효과는 Hunter 팀에서 ItemData_YHJ.effectType/value/effectDuration/usePercentValue를 읽어 적용 필요
                Debug.Log($"[Effect] Hunter 버프 처리 필요: {data.effectType}, value={data.value}, duration={data.effectDuration}");
                return false;

            default:
                Debug.Log($"[Effect] 미구현: {data.effectType}");
                return false;
        }
    }
}
