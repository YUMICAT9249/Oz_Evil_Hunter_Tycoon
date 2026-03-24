using UnityEngine;

[CreateAssetMenu(menuName = "Unit Data")]
public class UnitData_JBJ_PJS : ScriptableObject
{
    [Header("헌터 상세 스텟")]
    // 헌터 스텟 채우는 부분

    [Header("몬스터 상세 스텟")]
    public float maxHp = 100f;              // 몬스터 체력
    public float attackDamage = 10f;        // 몬스터 공격력
    public float moveSpeed = 0.5f;          // 몬스터 이동 속도
    public float detectRange = 5f;          // 몬스터 감지 범위
    public float attackRange = 1.2f;        // 몬스터 공격 범위
    public float attackCooldown = 1.5f;     // 몬스터 공격 쿨타임
}
