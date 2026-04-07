using UnityEngine;

/// <summary>
/// DataManager - 게임 데이터 테이블 총괄
/// 
/// 역할:
/// - ScriptableObject, DropTable, HunterData 등 모든 게임 데이터를 로드
/// - Manager_KJG.Data 형태로만 접근 가능
/// </summary>
public class DataManager_KJG : BaseManager_KJG<DataManager_KJG>
{
    [Header("=== 데이터 로드 상태 ===")]
    [SerializeField] private bool isDataLoaded = false;

    public bool IsDataLoaded => isDataLoaded;

    public event System.Action OnDataInitialized;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [DataManager_KJG] 데이터 매니저 초기화 완료");
    }

    /// <summary>
    /// GameManager에서 호출되는 데이터 초기화 메서드
    /// BaseManager의 virtual Initialize()를 override 함
    /// </summary>
    public override void Initialize()        // ← 여기서 override 추가!
    {
        Debug.Log("📊 [DataManager_KJG] 모든 데이터 테이블 로드 시작...");

        LoadAllDataTables();

        isDataLoaded = true;

        Debug.Log("✅ [DataManager_KJG] 데이터 로드 완료");

        OnDataInitialized?.Invoke();
        Manager_KJG.Event.RefreshUI();
    }

    private void LoadAllDataTables()
    {
        // TODO: 나중에 DropTableSO, HunterData, MonsterData 등을 Resources.LoadAll로 로드
        Debug.Log("[DataManager_KJG] 데이터 테이블 로드 완료 (현재는 기본 상태)");
    }

    public void SaveData()
    {
        Debug.Log("[DataManager_KJG] 데이터 저장 처리 완료");
    }
}