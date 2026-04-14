using System.Collections;
using UnityEngine;

// 헌터 스킬 계산식 / 스킬마다 개별 함수 / 스킬 추가 시 함수 추가 switch문에 추가
// 배율 값은 인스펙터에서 설정한다.
// 연타발생 스킬은 코루틴으로 관리한다.

public class HunterSkill_PJS : MonoBehaviour
{
    private HunterData_PJS _hunterData;
    private Animator _animator;

    [Header("보유 스킬 2개 등록")]
    public HunterSkillData_PJS _mainSkill;
    public HunterSkillData_PJS _subSkill;

    // 아래 쿨타임 제거 -> SO 빼올거 캐싱
    private float _mainSkillCooldown; // 1차 메인 스킬 쿨타임
    private float _subSkillCooldown;  // 1차 서브 스킬 쿨타임 
    private Battle_JBJ_PJS _battleTarget;

    void Awake()
    {
        _hunterData = GetComponent<HunterData_PJS>();
        _animator = GetComponent<Animator>();
    }

    public void UseSkill(Battle_JBJ_PJS target)
    { 
        // 쿨타임 체크
        if (target == null) return;
        _battleTarget = target;

        // 메인 스킬 1순위
        if (_mainSkill != null && Time.time >= _mainSkillCooldown)
        {
            _animator.SetTrigger(_mainSkill.skillName.ToString());
            _mainSkillCooldown = Time.time + _mainSkill.cooldownTime;
            SkillActive();
            return;
        }

        // 서브 스킬 2순위
        if (_subSkill != null && Time.time >= _subSkillCooldown)
        {
            _animator.SetTrigger(_subSkill.skillName.ToString());
            _subSkillCooldown = Time.time + _subSkill.cooldownTime;
            SkillBuff();
            return;
        }
    }

    // 1차 1번 스킬
    public void SkillActive()
    {
        if (_mainSkill == null) return;
        StopAllCoroutines();

        // 이펙트 재생
        PlaySkillEffect(_mainSkill);

        float ratio = (float)_mainSkill.currentLevel / _mainSkill.mainSkillMaxLevel;
        switch (_mainSkill.skillName)
        {
            case SkillName.Fury:
                Fury(ratio);
                break;
        
            case SkillName.HolyLight:
                HolyLight(ratio);
                break;
            
            case SkillName.MultiShot:
                StartCoroutine(MultiShotRoutine(ratio));
                break;
      
            case SkillName.ThunderBolt:
                StartCoroutine(ThunderBoltRoutine(ratio));
                break;
        }
    }

    // 1차 2번 스킬
    public void SkillBuff()
    {
        if (_subSkill == null) return;

        // 이펙트 재생
        PlaySkillEffect(_subSkill);

        float ratio = (float)_subSkill.currentLevel / _subSkill.subSkillMaxLevel;
        switch (_subSkill.skillName)
        {
            case SkillName.WarCry:
                WarCry(ratio);
                break;

            case SkillName.Barrier:
                Barrier(ratio);
                break;

            case SkillName.Dodge:
                Dodge(ratio);
                break;

            case SkillName.IceArmor:
                IceArmor(ratio);
                break;
        }
    }

    // 버프 종료
    public void SkillEnd()
    {
        // 중복 방지
        CancelInvoke();
        // 버프 이전으로 초기화
        _hunterData.FinalStats();
    }

    private void Fury(float ratio) // 버서커 1차 메인 퓨리
    {
        _hunterData._attackCooldown *= 0.25f;
        _hunterData._damage += _hunterData._damage * 1.0f * ratio;
        Invoke(nameof(SkillEnd), _mainSkill.durationTime);
    }

    private void HolyLight(float ratio) // 팔라딘 1차 메인 홀리라이트
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _mainSkill.splashRange);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Monster") && hits[i].TryGetComponent(out Battle_JBJ_PJS battle))
            {
                Damage(battle, _mainSkill.damageMultiplier * ratio);
            }
        }
    }

    IEnumerator MultiShotRoutine(float ratio) // 레인저 1차 메인 멀티샷
    {
        int count = _mainSkill.hitCount;
        for (int i = 0; i < count; i++)
        {
            if (_battleTarget != null)
            {
                Damage(_battleTarget, _mainSkill.damageMultiplier * ratio);
            }
            yield return new WaitForSeconds(_mainSkill.hitInterval);
        }
    }

    IEnumerator ThunderBoltRoutine(float ratio) // 소서러 1차 메인 썬더볼트
    {
        int count = _mainSkill.hitCount;
        for (int i = 0; i < count; i++)
        {
            if (_battleTarget != null)
            {
                Damage(_battleTarget, _mainSkill.damageMultiplier * ratio);
            }
            yield return new WaitForSeconds(_mainSkill.hitInterval);
        }
    }

    private void WarCry(float ratio)
    {
        _hunterData._damage += (_hunterData._damage * 1.0f * ratio);
        Invoke(nameof(EndWarCry), _subSkill.durationTime);
    }

    private void Barrier(float ratio)
    {
        _hunterData._defence += (_hunterData._defence * 0.6f * ratio);
        Invoke(nameof(EndBarrier), _subSkill.durationTime);
    }

    private void Dodge(float ratio)
    {
        _hunterData._dodgeChance += (0.3f * ratio);
        Invoke(nameof(EndDodge), _subSkill.durationTime);
    }

    private void IceArmor(float ratio)
    {
        _hunterData._defence += (_hunterData._defence * 0.5f * ratio);
        Invoke(nameof(EndIceArmor), _subSkill.durationTime);
    }

    #region 버프 종료 함수 / 수치 복구
    private void EndWarCry() { SkillEnd(); }
    private void EndBarrier() { SkillEnd(); }
    private void EndDodge() { SkillEnd(); }
    private void EndIceArmor() { SkillEnd(); }
    #endregion

    private void Damage(Battle_JBJ_PJS battleTarget, float ratio)
    {
        float finalDamage = _hunterData.GetAttackDamage() * ratio;
        battleTarget.TakeDamage(finalDamage, gameObject);
    }

    private void PlaySkillEffect(HunterSkillData_PJS hunterSkillData)
    { 
        if (hunterSkillData == null || hunterSkillData.effectPrefabs == null) return;

        if (Manager_KJG.Effect != null)
        {
            Manager_KJG.Effect.PlayEffect(hunterSkillData.effectPrefabs.name, transform.position);
        }
    }
}