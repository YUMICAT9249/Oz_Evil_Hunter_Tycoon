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

    // ★ YHJ TODO: HunterData_PJS.SetWeapon/SetArmor/SetGloves/SetBoots에서
    // itemID로 ItemData_YHJ를 가져온 뒤 itemKind, equipmentSlot, equipmentHunterJop을 먼저 검사해서 장착 가능 여부를 확인할 것
    public bool IsEquipmentItem()
    {
        return itemKind == ItemKind_YHJ.Equipment;
    }

    // ★ YHJ TODO: 헌터 직업과 장비 직업 제한이 맞는지 확인하는 공용 체크 함수
    public bool CanEquip(HunterJop hunterJop)
    {
        if (!IsEquipmentItem())
            return false;

        if (equipmentHunterJop == EquipmentHunterJop_YHJ.Public)
            return true;

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

    // ★ YHJ TODO: HunterData의 슬롯별 Set 함수에서 현재 슬롯과 장비 슬롯이 일치하는지 검사할 것
    public bool IsMatchSlot(EquipmentSlot_YHJ slot)
    {
        return IsEquipmentItem() && equipmentSlot == slot;
    }

    // ★ YHJ TODO: HunterData_PJS의 EquipStat에 그대로 복사해서 쓰기 위한 장비 수치 변환 함수
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
