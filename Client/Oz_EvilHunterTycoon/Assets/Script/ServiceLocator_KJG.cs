using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Service Locator (매니저 중개자)
/// 
/// 역할:
/// - 모든 매니저를 중앙에서 등록하고 안전하게 제공
/// - Manager_KJG가 내부에서 이 클래스를 통해 매니저를 가져감
/// - null 안전 처리 + 자세한 로그 추가로 디버깅이 매우 쉬워짐
/// 
/// 왜 이렇게 만들었나?
/// - 매니저들이 서로를 직접 알지 못하게 해서 결합도를 낮춤
/// - 등록이 안 된 매니저를 호출하면 친절하게 오류를 알려줌
/// </summary>
public class ServiceLocator_KJG : BaseManager_KJG<ServiceLocator_KJG>
{
    // 모든 매니저를 Type으로 저장 (키 = 매니저 타입, 값 = 실제 인스턴스)
    private readonly Dictionary<Type, MonoBehaviour> services = new Dictionary<Type, MonoBehaviour>();

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [ServiceLocator_KJG] 서비스 로케이터 초기화 완료");
    }

    /// <summary>
    /// 매니저를 Service Locator에 등록
    /// Bootstrapper에서 모든 매니저를 한 번에 등록할 때 사용
    /// </summary>
    public void Register<T>(T service) where T : MonoBehaviour
    {
        Type type = typeof(T);

        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"[ServiceLocator_KJG] {type.Name}은(는) 이미 등록되어 있습니다. (중복 등록 무시)");
            return;
        }

        services[type] = service;
        Debug.Log($"📌 [ServiceLocator_KJG] {type.Name} 등록 완료");
    }

    /// <summary>
    /// 매니저를 안전하게 가져오는 핵심 메서드
    /// Manager_KJG 내부에서만 호출됨
    /// </summary>
    public T Get<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);

        if (services.TryGetValue(type, out MonoBehaviour service))
        {
            return service as T;
        }

        // 오류 발생 시 친절하게 알려줌 (디버깅에 매우 유용)
        Debug.LogError($"[ServiceLocator_KJG] 오류! {type.Name}을(를) 찾을 수 없습니다.\n" +
                       "→ Bootstrapper_KJG에서 Register가 제대로 되었는지 확인하세요.");
        return null;
    }

    /// <summary>
    /// 모든 등록된 매니저를 초기화 (필요할 때만 사용)
    /// </summary>
    public void Clear()
    {
        services.Clear();
        Debug.Log("[ServiceLocator_KJG] 모든 서비스 초기화됨");
    }
}