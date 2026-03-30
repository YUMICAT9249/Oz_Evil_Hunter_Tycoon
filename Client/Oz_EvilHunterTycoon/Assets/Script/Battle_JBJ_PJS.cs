using UnityEngine;
using System;

// 헌터 몬스터 통합 전투 로직

public class Battle_JBJ_PJS : MonoBehaviour
{
    private HunterData_PJS _hunterData;
    private HunterController_PJS _hunterController;
    private Monster_JBJ _monsterController;

    // 누가, 누굴, float 만큼의 데미지로 때렸나
    public static Action<GameObject, GameObject, float> UnitAttack;

    void Awake()
    {
        _hunterData = GetComponent<HunterData_PJS>();
        _hunterController = GetComponent<HunterController_PJS>();
        _monsterController = GetComponent<Monster_JBJ>();

        if (_hunterData == null)
        {
            Debug.LogError("HunterData 연결 안됨", gameObject);
        }

        if (_hunterController == null)
        {
            Debug.LogError("HunterController 연결 안됨", gameObject);
        }

        if (_monsterController == null)
        {
            Debug.LogError("MonsterController 연결 안됨", gameObject);
        }
    }
    
    public void TakeDamage()
    { 
    
    }

    public void GiveDamage()
    { 
    
    }

    // 구독
    void OnEnable()
    {
        UnitAttack += AttackEvent;
    }

    // 구독 해제
    void OnDisable()
    {
        UnitAttack -= AttackEvent;
    }

    private void AttackEvent(GameObject attacker, GameObject target, float damage)
    {

    }

    private void FinalDamage(float damage, GameObject attacker)
    { 
    
    }

    private void Die()
    { 
    
    }
}
