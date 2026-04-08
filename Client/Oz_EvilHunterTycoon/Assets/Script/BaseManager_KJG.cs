using UnityEngine;

/// <summary>
/// BaseManager_KJG
/// 
/// 모든 _KJG 매니저가 상속받는 부모 클래스
/// Awake, Start, OnDestroy를 안전하게 상속받을 수 있도록 설계
/// </summary>
public abstract class BaseManager_KJG<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{typeof(T).Name}] 중복 생성 감지 → 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"✅ [{typeof(T).Name}] 초기화 완료");
    }

    /// <summary>
    /// 자식 클래스에서 Start()를 override할 수 있게 virtual로 추가
    /// </summary>
    protected virtual void Start()
    {
        // 기본적으로 아무것도 하지 않음. 자식 클래스에서 필요하면 override
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public virtual void Initialize() { }
}