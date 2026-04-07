using UnityEngine;

/// <summary>
/// BaseWorldObject_KJG
/// 
/// 역할:
/// - Monster, Hunter, Building 등 맵 오브젝트의 공통 베이스 클래스
/// - HP Bar는 **항상** 떠있게 관리
/// - 클릭하면 **추가 선택 UI 버튼**이 나타나도록 이벤트 발생
/// 
/// 사용 방법:
/// Monster_JBJ.cs, HunterController_PJS.cs, Building 스크립트에서 
/// "public class Monster_JBJ : BaseWorldObject_KJG"처럼 상속받기만 하면 됩니다.
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

    // HP Bar는 항상 떠있게 하기 위해 HP 변경 시 이벤트 발생
    public virtual void OnHealthChanged(float current, float max)
    {
        currentHp = current;
        Manager_KJG.Map.OnHealthChanged(this, currentHp, maxHp);
    }

    // 클릭하면 추가 선택 UI 버튼이 나타나도록 이벤트 발생
    public virtual void OnClicked()
    {
        Debug.Log($"[BaseWorldObject_KJG] {displayName}이(가) 클릭되었습니다.");
        Manager_KJG.Map.OnObjectClicked(this);
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