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
    private GameObject mainSkillObject;
    private GameObject subSkillObject;

    void Awake()
    {
        _hunterData = GetComponent<HunterData_PJS>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (_mainSkill == null) return;

        mainSkillObject = Instantiate(_mainSkill.effectPrefabs, transform);

        if (mainSkillObject != null)
        {
            mainSkillObject.SetActive(false);
        }

        if (_subSkill == null) return;

        subSkillObject = Instantiate(_subSkill.effectPrefabs, transform);
        
        if (subSkillObject != null)
        { 
            subSkillObject.SetActive(false);
        }
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

        // 사운드 재생
        PlaySkillSound(_mainSkill);

        if (Manager_KJG.Audio != null)
        {
            Manager_KJG.Audio.PlaySFX(_mainSkill.skillName.ToString());
        }

        float ratio = (float)_mainSkill.currentLevel / _mainSkill.mainSkillMaxLevel;
        switch (_mainSkill.skillName)
        {
            case SkillName.Fury:
                StartCoroutine(Fury(ratio));
                break;
        
            case SkillName.HolyLight:
                StartCoroutine(HolyLight(ratio));
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

        // 사운드 재생
        PlaySkillSound(_subSkill);

        if (Manager_KJG.Audio != null)
        {
            Manager_KJG.Audio.PlaySFX(_subSkill.skillName.ToString());
        }

        float ratio = (float)_subSkill.currentLevel / _subSkill.subSkillMaxLevel;
        subSkillObject.SetActive(true);
        
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
        subSkillObject.SetActive(false);
    }

    IEnumerator Fury(float ratio) // 버서커 1차 메인 퓨리
    {
        mainSkillObject.SetActive(true);
        _hunterData._attackCooldown *= 0.25f;
        _hunterData._damage += _hunterData._damage * 1.0f * ratio;

        yield return new WaitForSeconds(_mainSkill.durationTime);
        mainSkillObject.SetActive(false);
    }

    IEnumerator HolyLight(float ratio) // 팔라딘 1차 메인 홀리라이트
    {
        mainSkillObject.SetActive(true);
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _mainSkill.splashRange);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Monster") && hits[i].TryGetComponent(out Battle_JBJ_PJS battle))
            {
                Damage(battle, _mainSkill.damageMultiplier * ratio);
            }
        }
        yield return new WaitForSeconds(_mainSkill.durationTime);
        mainSkillObject.SetActive(false);
    }

    IEnumerator MultiShotRoutine(float ratio) // 레인저 1차 메인 멀티샷
    {
        // 기준 - 이펙트 위치 기준
        Vector3 direction = GetComponent<HunterController_PJS>()._targetMonster.transform.position - transform.position;
        // Z축 회전값 계산
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // 프리팹 기준 좌 -> 우
        mainSkillObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        mainSkillObject.SetActive(true);

        int count = _mainSkill.hitCount;
        for (int i = 0; i < count; i++)
        {
            if (_battleTarget != null)
            {
                Damage(_battleTarget, _mainSkill.damageMultiplier * ratio);
            }
            yield return new WaitForSeconds(_mainSkill.hitInterval);
        }
        yield return new WaitForSeconds(_mainSkill.durationTime);
        mainSkillObject.SetActive(false);
    }

    IEnumerator ThunderBoltRoutine(float ratio) // 소서러 1차 메인 썬더볼트
    {
        mainSkillObject.transform.position = GetComponent<HunterController_PJS>()._targetMonster.transform.position;
        mainSkillObject.SetActive(true);
        int count = _mainSkill.hitCount;
        for (int i = 0; i < count; i++)
        {
            if (_battleTarget != null)
            {
                Damage(_battleTarget, _mainSkill.damageMultiplier * ratio);
            }
            yield return new WaitForSeconds(_mainSkill.hitInterval);
        }
        yield return new WaitForSeconds(_mainSkill.durationTime);
        mainSkillObject.SetActive(false);
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

    private void PlaySkillSound(HunterSkillData_PJS skillData)
    {
        if (skillData == null) return;

        if (Manager_KJG.Audio != null && !string.IsNullOrEmpty(skillData.soundId))
        {
            Manager_KJG.Audio.PlaySFX(skillData.soundId);
        }
    }
}