using UnityEngine;

public enum ItemKind_YHJ
{
    Bandage,      // 붕대 (치료소)
    Potion,       // 포션 (전투)
    Equipment,    // 장비
    Material      // 재료 (드랍/제작)
}
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

    public ItemKind_YHJ itemKind;

    public ItemEffectType_YHJ effectType;
    public float value;

    [Header("Stack")]
    public bool isStackable = true;

    [Header("Equipment Stat")]
    public int attack;
    public int defense;
}