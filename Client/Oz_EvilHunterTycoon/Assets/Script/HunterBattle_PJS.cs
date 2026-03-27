using UnityEngine;

// 헌터의 전투 로직

public class HunterBattle_PJS : MonoBehaviour
{
    // [1] 참조
    private HunterData_PJS _hunterData;
    private HunterController_PJS _controller;

    void Awake()
    {
        _hunterData = GetComponent<HunterData_PJS>();
        _controller = GetComponent<HunterController_PJS>();

        if (_hunterData == null)
        {
            Debug.LogError("HunterData 연결 안됨", gameObject);
        }
    }

    // [2] 헌터 피격
    public void TakeDamage(float monsterDamage)
    {
        if (_hunterData == null) return;
        if (_hunterData._currentHP <= 0) return;

        // 1. 방어력 적용
        float defence = _hunterData.GetDefence();
        float finalDamage = Mathf.Max(0, monsterDamage - defence);

        // 2. 체력 감소
        _hunterData._currentHP -= finalDamage;
        Debug.Log($"헌터 피격 / 데미지 :{finalDamage}, 남은 HP : {_hunterData._currentHP}");

        // 3. 사망 처리
        if (_hunterData._currentHP <= 0)
        {
            // 음수 처리 방지
            _hunterData._currentHP = 0;
            _controller.HunterDie();
        }
    }

    // [3] 헌터 타격
    public void GiveDamage(GameObject targetMonster)
    {
        if (_hunterData == null) return;

        // 헌터 최종 공격력 가져오기
        float hunterDamage = _hunterData.GetAttackDamage();
        // 몬스터 최종 공격력 가져오기
        //MonsterScript.TakeDamage(hunterDamage);
        /*
        if (target != null)
        {
            target.TakeDamage(hunterDamage);    
        }
        Debug.Log($"몬스터 {targetMonster.name}에게 {hunterDamage} 입힘");
        */
    }
}
