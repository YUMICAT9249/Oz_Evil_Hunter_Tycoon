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

public enum SkillName
{ 
    NONE, Fury, WarCry, HolyLight, Barrier, MultiShot, Dodge, ThunderBolt, IceArmor
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill/HunterSkill")]
public class HunterSkillData_PJS : ScriptableObject
{
    [Header("스킬 기본 정보")]
    public SkillName skillName; // 열거형 스킬 이름 // String으로 / Id로
    public SkillType skillType; // 열거형 중 선택
    public HunterJop hunterJop; // 직업마다 갖게 될 스킬

    [Header("스킬 현재 / MAX 레벨")]
    public int currentLevel = 1;       // 최소 스킬 레벨 1 
    public int mainSkillMaxLevel = 10; // 1차 1번 스킬 최대 10
    public int subSkillMaxLevel = 5;   // 1차 2번 스킬 최대 5

    [Header("쿨타임 / 지속시간")]
    public float cooldownTime;  // 스킬 쿨타임
    public float durationTime;  // 스킬 지속시간

    [Header("액티브 수치 설정")]
    public int hitCount;            // 타격 횟수
    public float damageMultiplier;  // 데미지 배율

    [Header("연타 간격")]
    public float hitInterval;       // 연타 간격

    [Header("스킬 범위")]
    public float splashRange;       // 광역 범위

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
