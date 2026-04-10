using UnityEngine;

/// <summary>
/// [KJG 아키텍처 핵심] BaseWorldObject_KJG
///
/// 역할:
/// - Monster, Hunter, Building 등 맵 위 모든 오브젝트의 공통 베이스 클래스
/// - MapManager_KJG에 자동 등록/제거
/// - HP 관리, 클릭 처리, HP Bar 연동 등 공통 기능을 한 곳에 모음
///
/// 사용 방법:
/// Monster_JBJ.cs나 HunterController_PJS.cs에서
/// "public class Monster_JBJ : BaseWorldObject_KJG" 처럼 상속받기만 하면 됩니다.
/// </summary>
public abstract class BaseWorldObject_KJG : MonoBehaviour, IWorldObject_KJG
{
    [Header("BaseWorldObject 공통 정보")]
    [Tooltip("화면에 표시될 이름 (예: 고블린 전사, 나무 오두막)")]
    public string displayName = "Unknown Object";

    [Tooltip("최대 체력")]
    public float maxHp = 100f;

    // 현재 체력 (자식 클래스에서 접근 가능)
    protected float currentHp;

    protected virtual void Awake()
    {
        currentHp = maxHp;
        Debug.Log($"[BaseWorldObject_KJG] {displayName} 초기화 완료");
    }

    /// <summary>
    /// Start()에서 MapManager가 준비될 때까지 안전하게 기다림
    /// (Bootstrapper와 Start() 타이밍 문제 완전 해결)
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
            // MapManager가 아직 준비되지 않았다면 1프레임 지연 후 다시 시도
            StartCoroutine(DelayedRegister());
        }
    }

    /// <summary>
    /// 1프레임 뒤에 MapManager를 다시 확인하는 코루틴
    /// </summary>
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

    // ==================== MapManager 등록 / 제거 ====================
    protected virtual void RegisterToMapManager()
    {
        if (Manager_KJG.Map != null)
        {
            Manager_KJG.Map.RegisterObject(this);
        }
    }

    protected virtual void UnregisterFromMapManager()
    {
        if (Manager_KJG.Map != null)
        {
            Manager_KJG.Map.UnregisterObject(this);
        }
    }

    // ==================== IWorldObject_KJG 구현 ====================
    public GameObject GameObject => gameObject;
    public string ObjectType => GetType().Name;
    public string DisplayName => displayName;
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    public virtual void OnClicked()
    {
        Debug.Log($"[BaseWorldObject_KJG] {displayName}이(가) 클릭되었습니다.");
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
            Debug.Log($"[BaseWorldObject_KJG] {displayName} 사망");
            Destroy(gameObject);
        }
    }
}