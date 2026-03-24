using UnityEngine;

public class HunterBattle_PJS : MonoBehaviour
{
    private HunterData_PJS _data;
    private HunterController_PJS _controller;

    void Awake()
    {
        
    }

    public void TakeDamage(float monsterDamage)
    {
        if (_data == null || _data.HP <= 0)
        {
            return;
        }
        // 1. 데미지 계산 / 공격력 방어력 1:1비율로 상쇄
        float finalDamage = Mathf.Max(0, monsterDamage - _data.Defence);

        // 2. 데이터 차감
        _data.HP -= finalDamage;
        Debug.Log($"{_data.Name} 피격 / 데미지 :{finalDamage}, 남은 HP : {_data.HP}");

        if (_data.HP <= 0)
        {
            _controller.HunterDie();
        }
    }

    public void GiveDamage(GameObject targetMonster)
    {
        if (targetMonster != null)
        {
            float hunterDamage = _data.Damage;
            //MonsterScript.TakeDamage(hunterDamage);
            Debug.Log($"몬스터 {targetMonster.name}에게 {hunterDamage} 입힘");
        }
    }
}
