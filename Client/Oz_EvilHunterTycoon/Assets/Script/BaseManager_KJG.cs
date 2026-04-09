using UnityEngine;

/// <summary>
/// [KJG 아키텍처 핵심] 실무용 제네릭 싱글톤 베이스 매니저
/// 
/// 모든 _KJG 매니저(예: CurrencyManager_KJG, SaveLoadManager_KJG 등)가 상속받는 부모 클래스입니다.
/// 
/// 왜 만들었나?
/// - Awake(), Instance, DontDestroyOnLoad 코드가 매니저마다 거의 똑같이 복사되어 있었음 → 유지보수 최악
/// - 이 BaseManager 하나로 모든 매니저의 공통 로직을 중앙에서 관리
/// - 나중에 매니저가 20개가 되어도 Awake() 코드를 다시 작성할 필요가 없음
/// - 실무에서 가장 표준적인 "Generic Singleton + Base Class" 패턴 적용
/// </summary>
public abstract class BaseManager_KJG<T> : MonoBehaviour where T : MonoBehaviour
{
    
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        // 1. 이미 다른 인스턴스가 존재하는지 체크 (중복 생성 방지)
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[{typeof(T).Name}] 중복 생성 감지 → 파괴합니다.");
            Destroy(gameObject);
            return;
        }

        // 2. 싱글톤 인스턴스 등록
        Instance = this as T;

        // 3. 씬이 바뀌어도 이 오브젝트가 파괴되지 않도록 설정 (DontDestroyOnLoad)
        DontDestroyOnLoad(gameObject);

        Debug.Log($"✅ [{typeof(T).Name}] 초기화 완료 → BaseManager_KJG 상속");
    }

    /// <summary>
    /// 오브젝트가 파괴될 때 호출 (씬 이동이나 종료 시 정리용)
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary>
    /// 매니저별로 추가로 하고 싶은 초기화 작업이 있다면 이 메서드를 override해서 사용
    /// (Awake() 이후에 GameManager 등에서 호출 가능)
    /// </summary>
    public virtual void Initialize() { }
}