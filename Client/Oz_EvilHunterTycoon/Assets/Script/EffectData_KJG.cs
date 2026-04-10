using UnityEngine;

/// <summary>
/// [KJG] EffectData_KJG
/// 
/// 팀원이 Inspector에서 쉽게 새 파티클을 추가할 수 있도록 만든 ScriptableObject
/// Resources/Effects 폴더 안에 만들어 사용
/// </summary>
[CreateAssetMenu(menuName = "KJG/Effect Data", fileName = "New Effect")]
public class EffectData_KJG : ScriptableObject
{
    [Tooltip("이펙트를 호출할 때 사용할 ID (예: monster_death, building_upgrade)")]
    public string effectId;

    [Tooltip("재생할 ParticleSystem Prefab")]
    public ParticleSystem prefab;

    [Tooltip("이펙트가 유지되는 시간 (초)")]
    public float duration = 2f;
}