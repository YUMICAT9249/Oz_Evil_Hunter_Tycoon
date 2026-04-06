using UnityEngine;

// ScriptableObject를 사용해서 직업에 맞는 스킬들만 골라 가져가게할 헌터 스킬 데이터 스크립트

public enum SkillType
{ 
    NONE, Active, Passive, Buff
}

public enum StatType
{ 
    NONE, Damage, Defence, AttackSpeed, Dodge, Critical
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/HunterSkill")]
public class HunterSkillData_PJS : ScriptableObject
{
    [Header("스킬 기본 정보")]
    public string skillName;    // 스킬 이름
    public SkillType skillType; // 열거형 중 선택
    public HunterJop hunterJop; // 직업마다 갖게 될 스킬

    [Header("스킬 MAX 레벨")]
    public int maxLevel;        // 1차스킬은 5 또는 10

    [Header("쿨타임 / 지속시간")]
    public float cooldownTime;  // 스킬 쿨타임
    public float durationTime;  // 스킬 지속시간

    [Header("액티브 수치 설정")]
    public int hitCount;            // 타격 횟수
    public float damageMultiplier;  // 데미지 배율

    [Header("패시브/버프 수치 설정")]
    public StatType targetStat;     // 증감할 스탯
    public float statBonus;         // 증감할 스탯 수치

    [Header("상태이상 발동 확률")]
    public float probability;

    [Header("적 상태이상 지속시간")]
    public float ccDurationTime;

    [Header("이펙트 프리팹")]
    public GameObject effectPrefabs;
}
