using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CurrencyManager_KJG : MonoBehaviour
{
    public static CurrencyManager_KJG Instance {  get; private set; }

    [SerializeField] private double gold = 0;
    [SerializeField] private long exp = 0;
    [SerializeField] private int cash = 0;

    public float goldMultiplier = 1f;
    public float expMultiplier = 1f;
    public float dropRateBouns = 0f;

    //다른곳에서 접근못하게 프라이빗으로 하되 프로퍼티로 값을 바꿀수 있게 구현
    public double Gold
    {
        get => gold;
        set => gold = Math.Max(0, value);  //음수 방지
    }

    public long Exp
    {
        get => exp;
        set => exp = Math.Max(0L, value);
    }

    public int Cash
    {
        get => cash;
        set => cash = Math.Max(0, value);
    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        CheatInput();
    }


    public void Initialize()
    {

        UpdateMultipliers();
    }

    public void AddGold(double amount)
    {
        gold += amount * goldMultiplier;
        Debug.Log($"gold Add: +{amount} → {gold:N0}");
    }

    public void ADDExp(long amount)
    {
        exp += (long)(amount * expMultiplier);
    }

    public bool SpendCash(int amount)
    {
        if (cash >= amount)
        {
            cash -= amount;
            return true;
        }
        return false;
    }

    // 배수 증감은 여기서
    public void UpdateMultipliers()
    {
        goldMultiplier = 1f;
    }

    // 치트 G키=골드 증가, C키= 캐시 증가
    private void CheatInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
            AddGold(100_000_000);

        if (Input.GetKeyDown(KeyCode.C))
            cash+=100_000;
    }
}
