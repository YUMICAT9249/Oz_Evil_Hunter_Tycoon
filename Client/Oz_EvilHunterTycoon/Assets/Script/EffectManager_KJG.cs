using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EffectManager_KJG
///
/// 역할:
/// - 모든 파티클 / Visual Effect를 중앙에서 관리
/// - 풀링(Pooling)으로 성능 최적화 (동시에 많은 이펙트가 나와도 안정적)
/// - 팀원이 Inspector에서 쉽게 새 이펙트를 추가할 수 있게 설계
///
/// 사용 예시 (팀원이 이렇게 호출):
/// Manager_KJG.Effect.PlayEffect("monster_death", transform.position);
/// </summary>
public class EffectManager_KJG : BaseManager_KJG<EffectManager_KJG>
{
    [Header("Effect Data 폴더 경로")]
    [SerializeField] private string effectDataPath = "Effects";

    // ID로 EffectData 빠르게 찾기
    private Dictionary<string, EffectData_KJG> effectDict = new Dictionary<string, EffectData_KJG>();

    // 풀링 (각 이펙트별로 ParticleSystem 큐 관리)
    private Dictionary<string, Queue<ParticleSystem>> effectPool = new Dictionary<string, Queue<ParticleSystem>>();

    protected override void Awake()
    {
        base.Awake();
        LoadAllEffects();
        Debug.Log("[EffectManager_KJG] 이펙트 매니저 초기화 완료");
    }

    // Resources 폴더에서 모든 EffectData_KJG 자동 로드
    private void LoadAllEffects()
    {
        effectDict.Clear();
        EffectData_KJG[] datas = Resources.LoadAll<EffectData_KJG>(effectDataPath);

        foreach (var data in datas)
        {
            if (!string.IsNullOrEmpty(data.effectId))
                effectDict[data.effectId] = data;
        }

        Debug.Log($"[EffectManager_KJG] {effectDict.Count}개의 이펙트 데이터를 로드했습니다.");
    }

    /// <summary>
    /// 지정된 위치에 이펙트를 재생합니다.
    /// </summary>
    public void PlayEffect(string effectId, Vector3 position)
    {
        if (!effectDict.TryGetValue(effectId, out EffectData_KJG data) || data.prefab == null)
        {
            Debug.LogWarning($"[EffectManager_KJG] 이펙트를 찾을 수 없습니다: {effectId}");
            return;
        }

        // 풀에서 가져오기
        if (!effectPool.ContainsKey(effectId))
            effectPool[effectId] = new Queue<ParticleSystem>();

        ParticleSystem ps = null;
        if (effectPool[effectId].Count > 0)
        {
            ps = effectPool[effectId].Dequeue();
        }
        else
        {
            // 풀에 없으면 새로 생성
            ps = Instantiate(data.prefab).GetComponent<ParticleSystem>();
        }

        ps.transform.position = position;
        ps.Play();

        // 일정 시간 후 풀에 반환
        StartCoroutine(ReturnToPool(ps, data.duration, effectId));
    }

    private IEnumerator ReturnToPool(ParticleSystem ps, float duration, string effectId)
    {
        yield return new WaitForSeconds(duration);
        if (ps != null)
        {
            ps.Stop();
            ps.gameObject.SetActive(false);
            effectPool[effectId].Enqueue(ps);
        }
    }
}