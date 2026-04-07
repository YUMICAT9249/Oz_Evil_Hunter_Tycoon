using UnityEngine;

/// <summary>
/// BaseWorldObject_KJG
/// 
/// 역할:
/// - 모든 맵 오브젝트의 공통 베이스
/// - 건설 모드 등에서 Manager_KJG.Map이 null일 수 있는 상황도 안전하게 처리
/// </summary>
public abstract class BaseWorldObject_KJG : MonoBehaviour
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

    protected virtual void Start()
    {
        RegisterToMapManager();
    }

    protected virtual void OnDestroy()
    {
        UnregisterFromMapManager();
    }

    // ==================== 안전한 등록 / 제거 ====================
    protected virtual void RegisterToMapManager()
    {
        if (Manager_KJG.Map == null)
        {
            Debug.LogWarning($"[BaseWorldObject_KJG] MapManager가 아직 초기화되지 않았습니다. ({displayName})");
            return;
        }
        Manager_KJG.Map.RegisterObject(this);
    }

    protected virtual void UnregisterFromMapManager()
    {
        if (Manager_KJG.Map == null)
        {
            // 이미 Manager가 없으면 무시 (건설 모드 취소/파괴 시 자주 발생)
            return;
        }
        Manager_KJG.Map.UnregisterObject(this);
    }

    // ==================== 공통 기능 ====================
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    public virtual void OnClicked()
    {
        if (Manager_KJG.Map == null) return;
        Manager_KJG.Map.TriggerObjectClicked(this);
    }

    public virtual void OnHealthChanged(float current, float max)
    {
        currentHp = current;
        if (Manager_KJG.Map == null) return;
        Manager_KJG.Map.TriggerHealthChanged(this, currentHp, maxHp);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        OnHealthChanged(currentHp, MaxHp);

        if (currentHp <= 0)
        {
            Debug.Log($"[BaseWorldObject_KJG] {displayName} 사망");
            Destroy(gameObject);
        }
    }
}