using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// 헌터 몬스터 통합 전투 로직

public class Battle_JBJ_PJS : MonoBehaviour
{
    // 최적화를 위한 변수처리
    private HunterController_PJS _hunterController;
    private HunterData_PJS _hunterData;
    private Monster_JBJ _monsterData;

    void Awake()
    {
        // 캐싱 - 최적화
        _hunterController = GetComponent<HunterController_PJS>();
        _hunterData = GetComponent<HunterData_PJS>();
        _monsterData = GetComponent<Monster_JBJ>();
    }

    // 타격 실행
    public void GiveDamage(GameObject attacker)
    {
        if (attacker == null) return;

        float damage = 0;

        // 헌터 공격 가져오기
        if (_hunterData != null) { damage = _hunterData.GetAttackDamage(); }
        // 몬스터 공격 가져오기
        else if (_monsterData != null) { damage = _monsterData.data.attackDamage; }
        
        // 데미지 음수처리 방지 및 최소 데미지 적용
        if (damage <= 0) { damage = 1; }
        FinalDamage(damage, gameObject);
    }

    // 타겟 지목
    private void AttackEvent(GameObject attacker, GameObject target, float damage)
    {
        if (target == gameObject)
        {
            // 맞았으면 최종 데미지 계산으로 넘김
            FinalDamage(damage, attacker);
        }
    }

    // 최종 데미지 계산 / HP 실제 차감
    private void FinalDamage(float damage, GameObject attacker)
    {
        // 헌터 맞을 때 데미지 계산
        if (_hunterData != null)
        {
            if (_hunterData._currentHP <= 0) return;
            // 방어력 적용
            float defence = _hunterData.GetDefence();
            float finalDamage = Mathf.Max(0, damage - defence);
            // 실제 HP 차감 / UI에서 빼내어 갈 것
            _hunterData.CurrentHp -= finalDamage;
            Debug.Log($"{gameObject.name}이 {attacker.name}에게 {finalDamage}만큼 피해 입음.");

            // 사망 체크
            if (_hunterData._currentHP <= 0)
            {
                _hunterData._currentHP = 0;
                Die();
            }
        }

        // 몬스터 맞을 때 데미지 계산
        else if (_monsterData != null)
        {
            if (_monsterData.currentHP <= 0) return;

            // 방어력 적용
            float finalDamage = Mathf.Max(0, damage);
            // 실제 HP 차감
            _monsterData.currentHP -= finalDamage;
            Debug.Log($"{gameObject.name}이 {attacker.name}에게 {finalDamage}만큼 피해 입음.");

            // 사망 체크
            if (_monsterData.currentHP <= 0)
            {
                _monsterData.currentHP = 0;
                Die();
            }
        }
    }

    // 사망 처리
    private void Die()
    {
        // 헌터사망 -> 연출
        if (_hunterController != null)
        {
            HunterData_PJS.OnHunterDie?.Invoke();
            _hunterController.HunterDie();
        }
        
        // 몬스터사망 -> 연출 / 프리팹 파괴
        else if (_monsterData != null)
        {
            _monsterData.Die();
        }
    }
}
