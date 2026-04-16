using UnityEngine;
using UnityEngine.Serialization;

public enum ItemKind_YHJ
{
    Bandage,
    Potion,
    Equipment,
    Material
}

public enum ItemEffectType_YHJ
{
    None,
    HealHP,
    Revive,
    Buff
}

public enum EquipmentSlot_YHJ
{
    None,
    Weapon,
    Armor,
    Gloves,
    Boots
}

public enum EquipmentHunterJop_YHJ
{
    Public,
    Ranger,
    Sorcerer,
    Berserker,
    Paladin
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

    [Header("Equipment")]
    public EquipmentSlot_YHJ equipmentSlot = EquipmentSlot_YHJ.None;
    public EquipmentHunterJop_YHJ equipmentHunterJop = EquipmentHunterJop_YHJ.Public;

    [Header("Equipment Stat")]
    public float hp;
    public float damage;
    public float defence;
    public float criticalChance;
    public float dodgeChance;
    public float attackCooldown;
    public float moveSpeed;

    [Header("Prefab")]
    [FormerlySerializedAs("weaponPrefab")]
    public GameObject itemPrefab;

    public GameObject weaponPrefab => itemPrefab;

    public bool IsEquipmentItem()
    {
        return itemKind == ItemKind_YHJ.Equipment;
    }

    public bool CanEquip(HunterJop hunterJop)
    {
        if (!IsEquipmentItem())
        {
            return false;
        }

        if (equipmentHunterJop == EquipmentHunterJop_YHJ.Public)
        {
            return true;
        }

        switch (equipmentHunterJop)
        {
            case EquipmentHunterJop_YHJ.Ranger:
                return hunterJop == HunterJop.Ranger;

            case EquipmentHunterJop_YHJ.Sorcerer:
                return hunterJop == HunterJop.Sorcerer;

            case EquipmentHunterJop_YHJ.Berserker:
                return hunterJop == HunterJop.Berserker;

            case EquipmentHunterJop_YHJ.Paladin:
                return hunterJop == HunterJop.Paladin;
        }

        return false;
    }

    public bool IsMatchSlot(EquipmentSlot_YHJ slot)
    {
        return IsEquipmentItem() && equipmentSlot == slot;
    }

    public EquipStat ToEquipStat()
    {
        EquipStat stat = new EquipStat();
        stat.hp = hp;
        stat.damage = damage;
        stat.defence = defence;
        stat.criticalChance = criticalChance;
        stat.dodgeChance = dodgeChance;
        stat.attackCooldown = attackCooldown;
        stat.moveSpeed = moveSpeed;
        return stat;
    }
}
