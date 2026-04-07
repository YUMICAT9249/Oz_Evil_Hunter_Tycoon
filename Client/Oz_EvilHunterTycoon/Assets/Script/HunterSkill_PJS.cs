using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSkill_PJS : MonoBehaviour
{
    private HunterData_PJS _hunterData;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [Header("보유 스킬 2개 등록")]
    public HunterSkillData_PJS _mainSkill;
    public HunterSkillData_PJS _subSkill;

    private float _mainSkillCooldown; // 1차 메인 스킬 쿨타임
    private float _subSkillCooldown;  // 1차 서브 스킬 쿨타임
    private Battle_JBJ_PJS _battleTarget;

    void Awake()
    {
        _hunterData = GetComponent<HunterData_PJS>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UseSkill(Battle_JBJ_PJS target)
    { 
        // 쿨타임 체크
        if (target == null) return;
        _battleTarget = target;

        if (_mainSkill != null)
        {
            if (Time.time >= _mainSkillCooldown)
            {
                _animator.SetTrigger(_mainSkill.skillName.ToString());
                _mainSkillCooldown = Time.time + _mainSkill.cooldownTime;
                return;
            }
        }

        if (_subSkill != null)
        {
            if (Time.time >= _subSkillCooldown)
            {
                _animator.SetTrigger(_subSkill.skillName.ToString());
                _subSkillCooldown = Time.time + _subSkill.cooldownTime;
                return;
            }
        }
    }

    // 1차 1번 스킬
    public void SkillActive()
    {
        if (_mainSkill == null) return;

        float ratio = (float)_mainSkill.currentLevel / _mainSkill.mainSkillMaxLevel;
        switch (_mainSkill.skillName)
        {
            case SkillName.Fury:
                _hunterData._attackCooldown *= 0.25f;
                _hunterData._damage += _hunterData._damage * 1.0f * ratio;
                _spriteRenderer.color = Color.red;
                break;
        
            case SkillName.HolyLight:
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _mainSkill.splashRange);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].CompareTag("Monster") && hits[i].TryGetComponent(out Battle_JBJ_PJS battle))
                    {
                        Damage(battle, 12.0f * ratio);
                    }
                }
                break;
            
            case SkillName.MultiShot:
                Damage(_battleTarget, _mainSkill.damageMultiplier * ratio);
                break;
      
            case SkillName.ThunderBolt:
                Damage(_battleTarget, _mainSkill.damageMultiplier * ratio);
                break;
        }
        _hunterData.FinalStats();
    }

    // 1차 2번 스킬
    public void SkillBuff()
    {
        if (_subSkill == null) return;

        float ratio = (float)_subSkill.currentLevel / _subSkill.subSkillMaxLevel;
        switch (_subSkill.skillName)
        {
            case SkillName.WarCry:
                _hunterData._damage += (_hunterData._damage * 1.0f * ratio);
                Invoke(nameof(EndWarCry), _subSkill.durationTime);
                break;

            case SkillName.Barrier:
                _hunterData._defence += (_hunterData._defence * 0.6f * ratio);
                break;

            case SkillName.Dodge:
                _hunterData._dodgeChance += (0.3f * ratio);
                break;

            case SkillName.IceArmor:
                _hunterData._defence += (_hunterData._defence * 0.5f * ratio);
                break;
        }
    }

    // 버프 종료
    public void SkillEnd()
    {
        // 퓨리 
        _spriteRenderer.color = Color.white;
        // 버프 이전으로 초기화
        _hunterData.FinalStats();
    }

    #region 버프 종료 함수 / 수치 복구
    private void EndFury() { }
    private void EndWarCry() { }
    private void EndBarrier() { }
    private void EndDodge() { }
    private void EndIceArmor() { }
    #endregion

    private void Damage(Battle_JBJ_PJS battleTarget, float ratio)
    {
        float finalDamage = _hunterData.GetAttackDamage() * ratio;
        battleTarget.TakeDamage(finalDamage, gameObject);
    }
}
