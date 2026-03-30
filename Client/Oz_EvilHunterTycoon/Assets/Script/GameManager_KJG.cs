using UnityEngine;

public class GameManager_KJG : MonoBehaviour
{
    public static GameManager_KJG Instance { get; private set; }

    [Header("매니저 참조")]
    [SerializeField] private SaveLoadManager_KJG saveLoadManager;
    [SerializeField] private DataManager_KJG dataManager;
    [SerializeField] private CurrencyManager_KJG currencyManager;
    [SerializeField] private DifficultyManager_KJG difficultyManager;

    // 게임 상태
    public bool IsGameStarted { get; private set; } = false;

    // ==================== C# Events ====================
    public event System.Action OnGameStart;
    public event System.Action OnGameOver;
    public event System.Action OnNewGameStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("✅ GameManager_KJG 초기화 완료");
    }

    private void Start()
    {
        InitializeAllManagers();
    }

    // ==================== 전체 매니저 초기화 ====================
    private void InitializeAllManagers()
    {
        // 1. 세이브 로드 (가장 먼저)
        if (saveLoadManager != null)
        {
            saveLoadManager.GameLoad();
        }

        // 2. 데이터 매니저 초기화 (테이블, 설정 등)
        if (dataManager != null)
        {
            dataManager.Initialize();
        }

        // 3. 난이도 초기화
        if (difficultyManager != null)
        {
            // 로드된 난이도 값이 이미 DifficultyManager에 적용되어 있음
        }

        // 4. 화폐 매니저 초기화 (로드된 값 + 난이도 배율 적용)
        if (currencyManager != null)
        {
            // 필요시 추가 초기화 로직
        }

        Debug.Log("✅ 모든 매니저 초기화 완료");
    }

    // ==================== 새 게임 시작 ====================
    public void StartNewGame()
    {
        if (saveLoadManager != null)
        {
            saveLoadManager.NewGameSetup();     // 새 게임 초기화
        }

        IsGameStarted = true;

        Debug.Log("🆕 새 게임을 시작합니다.");

        // 이벤트 발생
        OnNewGameStarted?.Invoke();
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.GameStart);

        // 필요하다면 Currency, Difficulty 등 초기값 재설정
    }

    // ==================== 게임 오버 ====================
    public void GameOver()
    {
        IsGameStarted = false;

        Debug.Log("💀 게임 오버");

        OnGameOver?.Invoke();
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.GameOver);

        // 저장 요청
        if (saveLoadManager != null)
            saveLoadManager.GameSave();
    }

    // ==================== 게임 일시정지 ====================
    public void PauseGame()
    {
        Time.timeScale = 0f;
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.GamePause);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.GameResume);
    }

    // ==================== 세이브 / 로드 직접 호출 ====================
    public void SaveGame() => saveLoadManager?.GameSave();
    public void LoadGame() => saveLoadManager?.GameLoad();

    // ==================== 매니저 참조 안전하게 가져오기 ====================
    public SaveLoadManager_KJG SaveLoadManager => saveLoadManager;
    public DataManager_KJG DataManager => dataManager;
    public CurrencyManager_KJG CurrencyManager => currencyManager;
    public DifficultyManager_KJG DifficultyManager => difficultyManager;
}