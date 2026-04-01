using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// 헌터 몬스터 통합 전투 로직

public class Battle_JBJ_PJS : MonoBehaviour
{
    // 누가, 누굴, float 만큼의 데미지로 때렸나
    public static Action<GameObject, GameObject, float> UnitAttack;

    // [1] 타격 실행
    public void GiveDamage(GameObject target)
    {
        if (target == null) return;

        float damage = 0;

        // 헌터 공격
        if (TryGetComponent(out HunterData_PJS hunterData))
        {
            damage = hunterData.GetAttackDamage();
        }

        /*
        // 몬스터 공격
        else if (TryGetComponent(out 몬스터데이터))
        {
        
        }
        */
        UnitAttack?.Invoke(gameObject, target, damage);
    }

    // [2] 구독
    void OnEnable()
    {
        UnitAttack += AttackEvent;
    }

    // [3] 구독 해제
    void OnDisable()
    {
        UnitAttack -= AttackEvent;
    }

    // [4] 타겟 지목
    private void AttackEvent(GameObject attacker, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            // 맞았으면 최종 데미지 계산으로 넘김
            FinalDamage(damage, attacker);
        }
    }

    // [5] 최종 데미지 계산 / HP 실제 차감
    private void FinalDamage(float damage, GameObject attacker)
    {
        // 헌터 데미지 계산
        if (TryGetComponent(out HunterData_PJS hunterData))
        {
            if (hunterData._currentHP <= 0) return;
            // 방어력 적용
            float defence = hunterData.GetDefence();
            float finalDamage = Mathf.Max(0, damage - defence);
            // 실제 HP 차감
            hunterData._currentHP -= finalDamage;
            Debug.Log($"{gameObject.name}이 {attacker.name}에게 {finalDamage}만큼 피해 입음. 남은 HP {hunterData._currentHP}");
            // 사망 체크
            if (hunterData._currentHP <= 0)
            {
                hunterData._currentHP = 0;
                Die();
            }
        }

        // 몬스터 데미지 계산




    }

    // [6] 사망 처리
    private void Die()
    {
        // 헌터사망 -> 연출
        if (TryGetComponent(out HunterController_PJS hunterController))
        {
            HunterData_PJS.OnHunterDie?.Invoke();
            hunterController.HunterDie();
        }
        /*
        // 몬스터사망 -> 연출 / 프리팹 파괴
        else if (TryGetComponent(out 몬스터데이터))
        { 
        
        }
        */
    }
}
