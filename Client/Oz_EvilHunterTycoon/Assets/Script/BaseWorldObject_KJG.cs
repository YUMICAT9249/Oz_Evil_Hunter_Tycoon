using UnityEngine;

/// <summary>
/// BaseWorldObject_KJG - 최종 안전 버전
/// </summary>
public abstract class BaseWorldObject_KJG : MonoBehaviour, IWorldObject_KJG
{
    [Header("BaseWorldObject 공통 정보")]
    [Tooltip("화면에 표시될 이름")]
    public string displayName = "Unknown Object";

    [Tooltip("최대 체력")]
    public float maxHp = 100f;

    protected float currentHp;

    protected virtual void Awake()
    {
        currentHp = maxHp;
        Debug.Log($"[BaseWorldObject_KJG] {displayName} 초기화 완료");
    }

    /// <summary>
    /// MapManager가 준비될 때까지 안전하게 기다린 후 등록
    /// </summary>
    protected virtual void Start()
    {
        if (Manager_KJG.Map != null)
        {
            RegisterToMapManager();
            Debug.Log($"[BaseWorldObject_KJG] {displayName} → MapManager에 정상 등록 완료");
        }
        else
        {
            // Bootstrapper 등록이 끝날 때까지 1프레임 지연
            StartCoroutine(DelayedRegister());
        }
    }

    private System.Collections.IEnumerator DelayedRegister()
    {
        yield return null;   // 1프레임 기다림

        if (Manager_KJG.Map != null)
        {
            RegisterToMapManager();
            Debug.Log($"[BaseWorldObject_KJG] {displayName} → 지연 등록 성공");
        }
        else
        {
            Debug.LogWarning($"[BaseWorldObject_KJG] {displayName} - MapManager 등록 실패 (Bootstrapper 확인 필요)");
        }
    }

    protected virtual void OnDestroy()
    {
        UnregisterFromMapManager();
    }

    protected virtual void RegisterToMapManager()
    {
        if (Manager_KJG.Map != null)
            Manager_KJG.Map.RegisterObject(this);
    }

    protected virtual void UnregisterFromMapManager()
    {
        if (Manager_KJG.Map != null)
            Manager_KJG.Map.UnregisterObject(this);
    }

    // ==================== IWorldObject_KJG ====================
    public GameObject GameObject => gameObject;
    public string ObjectType => GetType().Name;
    public string DisplayName => displayName;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    public virtual void OnClicked()
    {
        if (Manager_KJG.Map != null)
            Manager_KJG.Map.TriggerObjectClicked(this);
    }

    public virtual void OnHealthChanged(float current, float max)
    {
        currentHp = current;
        if (Manager_KJG.Map != null)
            Manager_KJG.Map.TriggerHealthChanged(this, currentHp, maxHp);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;
        OnHealthChanged(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }
}