using UnityEngine;

public enum ItemEffectType_YHJ
{
    None,
    HealHP,
    Revive,
    Buff
}

[CreateAssetMenu(fileName = "ItemData_YHJ", menuName = "YHJ/Item Data")]
public class ItemData_YHJ : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite icon;

    public ItemEffectType_YHJ effectType;
    public float value;
}