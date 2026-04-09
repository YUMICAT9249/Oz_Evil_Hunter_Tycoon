using UnityEngine;

public class ItemEffectProcessor_YHJ : MonoBehaviour
{
    public bool ApplyItem(string itemID, IUnit_YHJ unit)
    {
        var data = ItemDatabase_YHJ.Instance.Get(itemID);

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

            case ItemEffectType_YHJ.Revive:
                unit.Revive();
                return true;

            default:
                Debug.Log($"[Effect] 미구현: {data.effectType}");
                return false;
        }
    }
}