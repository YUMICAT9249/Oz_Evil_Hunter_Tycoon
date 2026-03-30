using System.Collections.Generic;
using UnityEngine;

public class DataManager_KJG : MonoBehaviour
{
    public static DataManager_KJG Instance { get; private set; }

    [Header("=== 데이터 로드 상태 ===")]
    [SerializeField] private bool isDataLoaded = false;     // 필드로 변경

    // 읽기 전용 프로퍼티
    public bool IsDataLoaded => isDataLoaded;

    // ==================== C# Events ====================
    public event System.Action OnDataInitialized;        // 데이터 초기화 완료 시

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("✅ DataManager_KJG 초기화 완료");
    }

    /// <summary>
    /// 모든 게임 데이터(테이블)를 초기화하고 로드합니다.
    /// GameManager에서 호출됨
    /// </summary>
    public void Initialize()
    {
        Debug.Log("📊 DataManager 데이터 초기화 시작...");

        LoadAllDataTables();

        isDataLoaded = true;

        Debug.Log("✅ DataManager 데이터 초기화 완료");

        // 이벤트 발생
        OnDataInitialized?.Invoke();

        // 글로벌 UI 새로고침 이벤트
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RefreshUI);
    }

    /// <summary>
    /// 실제 데이터 테이블들을 로드하는 메서드
    /// </summary>
    private void LoadAllDataTables()
    {
        // TODO: 나중에 드롭 테이블, 아이템 테이블, 퀘스트 테이블 등을 여기서 로드
        // LoadDropTable();
        // LoadItemTable();
        // LoadMonsterTable();

        Debug.Log("[DataManager] 데이터 테이블 로드 완료 (현재는 빈 상태)");
    }

    // ==================== 데이터 조회 예시 메서드 ====================
    // public ItemData GetItemData(int itemId) { ... }
    // public DropGroup GetDropGroup(string groupId) { ... }

    // ==================== 세이브 관련 ====================
    public void SaveData()
    {
        // 현재 DataManager에서 저장할 데이터가 있다면 여기에 작성
        Debug.Log("[DataManager] 데이터 저장 처리 완료");
    }
}