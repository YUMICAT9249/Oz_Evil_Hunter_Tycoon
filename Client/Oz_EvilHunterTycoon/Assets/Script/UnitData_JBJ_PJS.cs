using UnityEngine;

public enum Unit
{ 
    NONE,
    Hunter,
    Monster
}

[CreateAssetMenu(menuName = "Unit Data")]
public class UnitData_JBJ_PJS : ScriptableObject
{
    [Header("유닛 상세 스텟(공통)")]
    public float maxHp = 100f;              // 최대 체력
    public float attackDamage = 10f;        // 공격력
    public float attackCooldown = 1.5f;     // 공격 쿨타임
    public float moveSpeed = 0.5f;          // 이동 속도
    public float detectRange = 5f;          // 감지 범위
    public float attackRange = 1.2f;        // 공격 범위
    
    [Header("몬스터 상세 스텟")]
    // 몬스터 상세 스텟

    [Header("헌터 상세 스텟")]
    public string hunterName;              // 헌터 이름
    public HunterJop hunterJop;            // 헌터 직업
    public int maxLevel = 100;              // 최대 레벨

    public float defence = 10;           // 기본 방어력
    public float criticalChance = 10.0f; // 기본 치명타확률
    public float dodgeChance = 10.0f;    // 기본 회피확률
}
