using UnityEngine;

/// <summary>
/// BaseWorldObject_KJG
///
/// 역할:
/// - Monster, Hunter, Building 등 맵 위 모든 오브젝트의 공통 베이스 클래스
/// - MapManager_KJG에 자동 등록/제거
/// - HP 관리, 클릭 처리 등 공통 기능을 한 곳에 모음
/// </summary>
public abstract class BaseWorldObject_KJG : MonoBehaviour, IWorldObject_KJG
{
    [Header("BaseWorldObject 공통 정보")]
    [Tooltip("화면에 표시될 이름")]
    public string displayName = "Unknown Object";

    [Tooltip("최대 체력 (건물은 0으로 설정)")]
    public float maxHp = 100f;

    protected float currentHp;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    protected virtual void Start()
    {
        // ★★★ 매우 안전하게 MapManager 확인 (오류 방지)
        if (Manager_KJG.Map != null)
        {
            RegisterToMapManager();
        }
        else
        {
            Debug.LogWarning($"[BaseWorldObject_KJG] {displayName} - MapManager가 아직 초기화되지 않았습니다. (건설 모드에서 발생 가능)");
        }
    }

    protected virtual void OnDestroy()
    {
        // ★★★ OnDestroy에서도 안전하게 확인
        if (Manager_KJG.Map != null)
        {
            UnregisterFromMapManager();
        }
    }

    // ==================== MapManager 등록 / 제거 ====================
    protected virtual void RegisterToMapManager()
    {
        Manager_KJG.Map.RegisterObject(this);
    }

    protected virtual void UnregisterFromMapManager()
    {
        Manager_KJG.Map.UnregisterObject(this);
    }

    // ==================== IWorldObject_KJG 구현 ====================
    public GameObject GameObject => gameObject;
    public string ObjectType => GetType().Name;
    public string DisplayName => displayName;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    public virtual void OnClicked()
    {
        Manager_KJG.Map?.TriggerObjectClicked(this);
    }

    public virtual void OnHealthChanged(float current, float max)
    {
        currentHp = current;
        Manager_KJG.Map?.TriggerHealthChanged(this, currentHp, maxHp);
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