using UnityEngine;

/// <summary>
/// [KJG 실무 아키텍처] BaseWorldObject_KJG
/// 
/// 역할:
/// - Monster, Hunter, Building 등 맵 오브젝트의 공통 베이스 클래스
/// - MapManager_KJG에 자동 등록/제거
/// - HP 관리, 클릭 처리, TakeDamage 등 공통 기능을 한 곳에 모음
/// 
/// 사용 방법:
/// Monster_JBJ.cs에서 "public class Monster_JBJ : BaseWorldObject_KJG"처럼 상속받기만 하면 됩니다.
/// </summary>
public abstract class BaseWorldObject_KJG : MonoBehaviour
{
    [Header("BaseWorldObject 공통 정보")]
    [Tooltip("화면에 표시될 이름")]
    public string displayName = "Unknown Object";

    [Tooltip("최대 체력")]
    public float maxHp = 100f;          // ← maxHp 선언 (오류 원인)

    protected float currentHp;          // 현재 체력

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

    // ==================== MapManager 등록 / 제거 ====================
    protected virtual void RegisterToMapManager()
    {
        Manager_KJG.Map.RegisterObject(this);
    }

    protected virtual void UnregisterFromMapManager()
    {
        Manager_KJG.Map.UnregisterObject(this);
    }

    // ==================== 공통 기능 ====================
    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    public virtual void OnClicked()
    {
        Manager_KJG.Map.OnObjectClicked(this);
    }

    public virtual void OnHealthChanged(float current, float max)
    {
        currentHp = current;
        Manager_KJG.Map.OnHealthChanged(this, currentHp, maxHp);
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