using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSkill_PJS : MonoBehaviour
{
    [Header("스킬 데이터베이스 (전체 8개 리스트)")]
    public List<HunterSkillData_PJS> _hunterSkillDatabase = new List<HunterSkillData_PJS>();

    [Header("헌터가 보유한 스킬 (자동 세팅)")]
    public List<HunterSkillData_PJS> _hunterSkill = new List<HunterSkillData_PJS>();

    // 쿨타임 계산용
    private Dictionary<string, float> _lastSkillTime = new Dictionary<string, float>();

    private HunterData_PJS _hunterData;

    void Awake()
    {
        _hunterData = GetComponent<HunterData_PJS>();
    }

    void Start()
    {
        
    }

    // 데이터 베이스에서 직업에 맞는 스킬 골라서 담기
    private void GetHunterSkill()
    {
        if (_hunterData == null) return;

        for (int i = 0; i < _hunterSkillDatabase.Count; i++)
        { 
            
        }
    }
}
