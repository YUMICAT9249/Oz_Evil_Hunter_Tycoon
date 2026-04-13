using UnityEngine;

/// <summary>
/// GameManager - 게임 전체 흐름 총괄
/// 
/// 특징:
/// - Manager_KJG.Game 형태로만 접근
/// - 다른 매니저들은 Manager_KJG를 통해 접근 (직접 참조 제거)
/// - 게임 시작, 종료, 일시정지 등 전체 흐름을 관리
/// </summary>
public class GameManager_KJG : BaseManager_KJG<GameManager_KJG>
{
    [Header("매니저 참조 (Inspector에서 연결)")]
    [SerializeField] private SaveLoadManager_KJG saveLoadManager;
    [SerializeField] private DataManager_KJG dataManager;
    [SerializeField] private CurrencyManager_KJG currencyManager;
    [SerializeField] private DifficultyManager_KJG difficultyManager;

    // HunterManager는 팀원 스크립트이므로 나중에 연결
    // public HunterManager_PJS HunterManager { get; private set; }

    public bool IsGameStarted { get; private set; } = false;

    // ==================== C# Events ====================
    public event System.Action OnGameStart;
    public event System.Action OnGameOver;
    public event System.Action OnNewGameStarted;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [GameManager_KJG] 게임 매니저 초기화 완료");
    }

    protected override void Start()
    {
        base.Start();

        // Bootstrapper가 매니저 등록을 완료할 때까지 기다림
        if (Manager_KJG.SaveLoad != null)
        {
            InitializeAllManagers();
        }
        else
        {
            Debug.LogWarning("[GameManager_KJG] SaveLoad가 아직 준비되지 않았습니다.");
        }
    }

    private void InitializeAllManagers()
    {
        Debug.Log("📋 [GameManager_KJG] 모든 매니저 초기화 시작...");

        // SaveLoad가 가장 먼저 로드되어야 함
        Manager_KJG.SaveLoad.GameLoad();

        // 데이터 매니저 초기화
        if (Manager_KJG.Data != null)
            Manager_KJG.Data.Initialize();

        Debug.Log("✅ [GameManager_KJG] 모든 매니저 초기화 완료");
    }

    // ==================== 게임 시작 ====================
    public void StartNewGame()
    {
        if (Manager_KJG.SaveLoad != null)
            Manager_KJG.SaveLoad.NewGameSetup();

        IsGameStarted = true;

        Debug.Log("🆕 새 게임 시작");

        OnNewGameStarted?.Invoke();
        Manager_KJG.Event.Invoke(EventManager_KJG.GameEvent.GameStart);
    }

    // ==================== 게임 오버 ====================
    public void GameOver()
    {
        IsGameStarted = false;

        Debug.Log("💀 게임 오버");

        OnGameOver?.Invoke();
        Manager_KJG.Event.Invoke(EventManager_KJG.GameEvent.GameOver);

        Manager_KJG.SaveLoad.GameSave();
    }

    // ==================== 일시정지 ====================
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Manager_KJG.Event.Invoke(EventManager_KJG.GameEvent.GamePause);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Manager_KJG.Event.Invoke(EventManager_KJG.GameEvent.GameResume);
    }

    // ==================== 편의 메서드 ====================
    public void SaveGame() => Manager_KJG.SaveLoad?.GameSave();
    public void LoadGame() => Manager_KJG.SaveLoad?.GameLoad();
}