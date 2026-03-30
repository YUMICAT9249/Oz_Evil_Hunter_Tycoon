using UnityEngine;
using System;

public class CurrencyManager_KJG : MonoBehaviour
{
    public static CurrencyManager_KJG Instance { get; private set; }

    [Header("현재 자원")]
    [SerializeField] private double gold = 0;
    [SerializeField] private long exp = 0;
    [SerializeField] private int cash = 0;

    [Header("배율")]
    public float goldMultiplier = 1f;
    public float expMultiplier = 1f;

    // 읽기 전용 프로퍼티
    public double Gold => gold;
    public long Exp => exp;
    public int Cash => cash;

    // ==================== C# Event (로컬 이벤트) ====================
    public event Action<double> OnGoldChanged;
    public event Action<long> OnExpChanged;
    public event Action<int> OnCashChanged;
    public event Action OnUIRefreshRequested;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ CurrencyManager_KJG 초기화 완료");
    }

    // ==================== 화폐 증감 메서드 ====================
    public void AddGold(double amount)
    {
        if (amount <= 0) return;

        gold += amount * goldMultiplier;
        gold = Math.Max(0, gold);

        OnGoldChanged?.Invoke(amount);           // Currency 전용 이벤트
        RequestUIRefreshAndSave();
    }

    public void AddExp(long amount)
    {
        if (amount <= 0) return;

        exp += (long)(amount * expMultiplier);
        exp = Math.Max(0, exp);

        OnExpChanged?.Invoke(amount);
        RequestUIRefreshAndSave();
    }

    public void AddCash(int amount)
    {
        if (amount == 0) return;

        cash += amount;
        cash = Mathf.Max(0, cash);

        OnCashChanged?.Invoke(amount);
        RequestUIRefreshAndSave();
    }

    public bool SpendCash(int amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            OnCashChanged?.Invoke(-amount);
            RequestUIRefreshAndSave();
            return true;
        }

        Debug.LogWarning("캐시가 부족합니다!");
        return false;
    }

    private void RequestUIRefreshAndSave()
    {
        OnUIRefreshRequested?.Invoke();

        // 올바른 호출 방식
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RequestSave);
        EventManager_KJG.Instance.Invoke(EventManager_KJG.GameEvent.RefreshUI);
    }

    // CurrencyManager_KJG.cs 에 추가할 메서드
    public void UpdateMultipliers(int difficultyLevel)
    {
        if (DifficultyManager_KJG.Instance != null)
        {
            goldMultiplier = DifficultyManager_KJG.Instance.GetCurrentGoldMultiplier();
            expMultiplier = DifficultyManager_KJG.Instance.GetCurrentExpMultiplier();

            Debug.Log($"[Currency] 난이도 변경으로 배율 업데이트 → Gold:{goldMultiplier:F2} Exp:{expMultiplier:F2}");
        }
    }

    // ==================== 치트 ====================
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            AddGold(100_000_000.0);

        if (Input.GetKeyDown(KeyCode.C))
            AddCash(100_000);
    }

    // 세이브/로드용
    public void SetGold(double value) => gold = Math.Max(0, value);
    public void SetExp(long value) => exp = Math.Max(0, value);
    public void SetCash(int value) => cash = Mathf.Max(0, value);
}