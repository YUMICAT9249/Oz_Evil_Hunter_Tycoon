using UnityEngine;
using System;

/// <summary>
/// 화폐 관리 매니저
/// 
/// 특징:
/// - Manager_KJG.Currency 형태로만 접근 가능
/// - 직접 .Instance 호출 완전 제거
/// - 이벤트는 Manager_KJG.Event를 사용
/// </summary>
public class CurrencyManager_KJG : BaseManager_KJG<CurrencyManager_KJG>
{
    [Header("현재 자원")]
    [SerializeField] private double gold = 0;
    [SerializeField] private long exp = 0;
    [SerializeField] private int cash = 0;

    [Header("배율")]
    public float goldMultiplier = 1f;
    public float expMultiplier = 1f;

    public double Gold => gold;
    public long Exp => exp;
    public int Cash => cash;

    public event Action<double> OnGoldChanged;
    public event Action<long> OnExpChanged;
    public event Action<int> OnCashChanged;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [CurrencyManager_KJG] 화폐 시스템 초기화 완료");
    }

    public void AddGold(double amount)
    {
        if (amount <= 0) return;
        gold += amount * goldMultiplier;
        gold = Math.Max(0, gold);

        OnGoldChanged?.Invoke(amount);
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
        Debug.LogWarning("캐시 부족!");
        return false;
    }

    private void RequestUIRefreshAndSave()
    {
        Manager_KJG.Event.RefreshUI();           // EventManager를 Manager_KJG를 통해 호출
        Manager_KJG.SaveLoad.GameSave();         // 저장 요청도 Manager_KJG를 통해
    }

    public void UpdateMultipliers(int difficultyLevel)
    {
        if (Manager_KJG.Difficulty != null)
        {
            goldMultiplier = Manager_KJG.Difficulty.GetCurrentGoldMultiplier();
            expMultiplier = Manager_KJG.Difficulty.GetCurrentExpMultiplier();
            Debug.Log($"[Currency] 난이도 배율 업데이트 → Gold:{goldMultiplier:F2} Exp:{expMultiplier:F2}");
        }
    }

    public void SetGold(double value) => gold = Math.Max(0, value);
    public void SetExp(long value) => exp = Math.Max(0, value);
    public void SetCash(int value) => cash = Mathf.Max(0, value);
}