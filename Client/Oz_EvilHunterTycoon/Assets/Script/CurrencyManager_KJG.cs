using System;
using UnityEngine;

/// <summary>
/// CurrencyManager_KJG
/// 원작의 Gold, Cash, Exp 시스템 완전 구현
/// UI 갱신, Save/Load, Building 소비와 완벽 연동
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

    // 이벤트 (UI 갱신용)
    public event Action<double> OnGoldChanged;
    public event Action<long> OnExpChanged;
    public event Action<int> OnCashChanged;

    public double Gold => gold;
    public long Exp => exp;
    public int Cash => cash;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("✅ [CurrencyManager_KJG] 경제 시스템 초기화 완료");
    }

    public void AddGold(double amount)
    {
        if (amount <= 0) return;
        gold += amount * goldMultiplier;
        gold = Math.Max(0, gold);
        OnGoldChanged?.Invoke(gold);
        RequestSaveAndUIUpdate();
    }

    public void AddExp(long amount)
    {
        if (amount <= 0) return;
        exp += (long)(amount * expMultiplier);
        exp = Math.Max(0, exp);
        OnExpChanged?.Invoke(exp);
        RequestSaveAndUIUpdate();
    }

    public void AddCash(int amount)
    {
        if (amount <= 0) return;
        cash += amount;
        OnCashChanged?.Invoke(cash);
        RequestSaveAndUIUpdate();
    }

    public bool SpendGold(double amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        RequestSaveAndUIUpdate();
        return true;
    }

    public bool SpendCash(int amount)
    {
        if (cash < amount) return false;
        cash -= amount;
        OnCashChanged?.Invoke(cash);
        RequestSaveAndUIUpdate();
        return true;
    }

    private void RequestSaveAndUIUpdate()
    {
        if (Manager_KJG.SaveLoad != null)
            Manager_KJG.SaveLoad.GameSave();

        if (Manager_KJG.Event != null)
            Manager_KJG.Event.RefreshUI();
    }

    // 난이도 변경 시 배율 업데이트
    public void UpdateMultipliers()
    {
        if (Manager_KJG.Difficulty != null)
        {
            goldMultiplier = Manager_KJG.Difficulty.GetCurrentGoldMultiplier();
            expMultiplier = Manager_KJG.Difficulty.GetCurrentExpMultiplier();
        }
    }

    // Save/Load용
    public void SetGold(double value) => gold = Math.Max(0, value);
    public void SetExp(long value) => exp = Math.Max(0, value);
    public void SetCash(int value) => cash = Mathf.Max(0, value);
}