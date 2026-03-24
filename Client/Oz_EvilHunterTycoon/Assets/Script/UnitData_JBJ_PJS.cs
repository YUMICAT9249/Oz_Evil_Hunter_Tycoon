using UnityEngine;

public class UnitData_JBJ_PJS : MonoBehaviour
{
    [Header("몬스터 상세 스텟")]
    [SerializeField] public float maxHp = 100f;              // 몬스터 체력
    [SerializeField] public float attackDamage = 10f;        // 몬스터 공격력
    [SerializeField] public float moveSpeed = 0.5f;          // 몬스터 이동 속도
    [SerializeField] public float detectRange = 5f;          // 몬스터 감지 범위
    [SerializeField] public float attackRange = 1.2f;        // 몬스터 공격 범위
    [SerializeField] public float attackCooldown = 1.5f;     // 몬스터 공격 쿨타임
}
