﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

// ★ 치료소 기능
public class HealInteraction_YHJ : MonoBehaviour, IBuildingInteraction_YHJ
{
    [Serializable]
    public class BandageOption
    {
        public string itemID;
        public float healAmount;
        public int goldCost;
        public string materialID = "Cloth";
        public int materialCost = 2;
    }

    private BuildingInventory_YHJ inventory;
    private BuildingQueue_YHJ queue;
    private BuildingLevelComponent_YHJ levelComponent;

    [SerializeField] private BandageOption[] bandages =
    {
        new BandageOption { itemID = "LinenBandage", healAmount = 8000f, goldCost = 60 },
        new BandageOption { itemID = "WoolBandage", healAmount = 29600f, goldCost = 180 },
        new BandageOption { itemID = "SilkBandage", healAmount = 59200f, goldCost = 540 }
    };

    void Awake()
    {
        inventory = GetComponent<BuildingInventory_YHJ>();
        queue = GetComponent<BuildingQueue_YHJ>();
        levelComponent = GetComponent<BuildingLevelComponent_YHJ>();
    }

    void OnEnable()
    {
        EventBus_YHJ.RequestProcessUnit += OnProcessUnit;
    }

    void OnDisable()
    {
        EventBus_YHJ.RequestProcessUnit -= OnProcessUnit;
    }

    public bool CanInteract(IUnit_YHJ unit)
    {
        if (unit == null || unit.IsDead)
            return false;

        float triggerHpPercent = 0.5f;

        if (levelComponent != null && levelComponent.CurrentStat != null)
        {
            if (levelComponent.CurrentStat.autoHealHpPercent > 0f)
                triggerHpPercent = levelComponent.CurrentStat.autoHealHpPercent * 0.01f;
        }

        return unit.CurrentHP <= unit.MaxHP * triggerHpPercent;
    }

    public void Interact(IUnit_YHJ unit)
    {
        if (unit == null || queue == null)
            return;

        // ★ 헌터 자동 이동 후 치료소 도착 시 대기열 등록
        queue.Enqueue(unit);

        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Queued
        );
    }

    private void OnProcessUnit(IUnit_YHJ unit, GameObject building)
    {
        if (building != gameObject)
            return;

        if (unit == null || unit.IsDead)
            return;

        BandageOption bandage = SelectBestBandage(unit);
        if (bandage == null)
        {
            Debug.Log("[Heal] 사용 가능한 붕대 없음");
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        if (!EnsureBandageStock(bandage))
        {
            Debug.Log($"[Heal] {bandage.itemID} 재고 없음 - 치료 대기 유지");

            // ★ YHJ: 치료소는 UI 제작으로 쌓인 붕대 재고가 있어야 처리되므로 재고가 생길 때까지 큐에 유지
            queue.Enqueue(unit);
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Queued);
            return;
        }

        EventBus_YHJ.RequestBuyItem?.Invoke(unit, bandage.itemID);

        if (!TrySpendUnitGold(unit, bandage.goldCost))
        {
            Debug.Log($"[Heal] 헌터 골드 부족: {bandage.goldCost}");
            EventBus_YHJ.OnBuyItemResult?.Invoke(unit, bandage.itemID, false);
            EventBus_YHJ.OnInteractionResult?.Invoke(unit, InteractionResult_YHJ.Fail);
            return;
        }

        EventBus_YHJ.OnBuyItemResult?.Invoke(unit, bandage.itemID, true);

        inventory.RemoveItem(bandage.itemID, 1);
        unit.Heal(GetBandageHealAmount(bandage));

        // ★ 아직 체력이 부족하면 다시 대기열
        if (unit.CurrentHP < unit.MaxHP)
        {
            queue.Enqueue(unit);
        }

        Manager_KJG.Audio?.PlaySFX("CD01042");
        EventBus_YHJ.OnInteractionResult?.Invoke
        (
            unit,
            InteractionResult_YHJ.Success
        );
    }

    private BandageOption SelectBestBandage(IUnit_YHJ unit)
    {
        if (unit == null || bandages == null || bandages.Length == 0)
            return null;

        float missingHp = Mathf.Max(0f, unit.MaxHP - unit.CurrentHP);

        List<BandageOption> usableBandages = bandages
            .Where(bandage => bandage != null)
            .Where(bandage => !string.IsNullOrEmpty(bandage.itemID))
            .Where(bandage => levelComponent == null || levelComponent.CanUseItem(bandage.itemID))
            .ToList();

        if (usableBandages.Count == 0)
            return null;

        BandageOption bestEnoughBandage = usableBandages
            .Where(bandage => GetBandageHealAmount(bandage) >= missingHp)
            .OrderBy(bandage => GetBandageHealAmount(bandage))
            .FirstOrDefault();

        if (bestEnoughBandage != null)
            return bestEnoughBandage;

        return usableBandages
            .OrderByDescending(bandage => GetBandageHealAmount(bandage))
            .FirstOrDefault();
    }

    // ★ YHJ: 붕대 치료량은 아이템 자체 효과값을 우선 사용하고, 데이터가 비어 있으면 기존 치료소 설정값을 보조로 사용
    private float GetBandageHealAmount(BandageOption bandage)
    {
        if (bandage == null)
            return 0f;

        if (ItemDatabase_YHJ.Instance != null)
        {
            ItemData_YHJ itemData = ItemDatabase_YHJ.Instance.Get(bandage.itemID);

            if (itemData != null && itemData.effectType == ItemEffectType_YHJ.HealHP && itemData.value > 0f)
                return itemData.value;
        }

        return bandage.healAmount;
    }

    // ★ YHJ: 치료 처리에서는 자동 제작하지 않고 치료소 인벤토리 재고만 확인
    private bool EnsureBandageStock(BandageOption bandage)
    {
        if (bandage == null || inventory == null)
            return false;

        return inventory.HasItem(bandage.itemID, 1);
    }

    // ★ YHJ: 치료소 UI 생산 버튼에서 호출할 수 있도록 붕대 제작 진입점 분리
    public bool TryProduceBandageForUI(string itemID)
    {
        if (string.IsNullOrEmpty(itemID) || bandages == null)
            return false;

        BandageOption bandage = bandages
            .FirstOrDefault(option => option != null && option.itemID == itemID);

        return TryMakeBandage(bandage);
    }

    // ★ YHJ: UI 제작 성공 시 치료소 인벤토리에 붕대 재고 저장
    private bool TryMakeBandage(BandageOption bandage)
    {
        if (bandage == null)
            return false;

        if (levelComponent != null && !levelComponent.CanUseItem(bandage.itemID))
            return false;

        if (MaterialInventory_YHJ.Instance == null)
            return false;

        ItemRecipe_YHJ recipe = ItemRecipeDatabase_YHJ.Instance?.GetByItemID(bandage.itemID);

        if (recipe != null)
        {
            if (!recipe.TryConsume(MaterialInventory_YHJ.Instance, 1, 0))
                return false;
        }
        else
        {
            if (!MaterialInventory_YHJ.Instance.HasItem(bandage.materialID, bandage.materialCost))
                return false;

            MaterialInventory_YHJ.Instance.RemoveItem(bandage.materialID, bandage.materialCost);
        }

        inventory.AddItem(bandage.itemID, 1);

        // ★ UI 제작 완료 연결
        // EventBus_YHJ.OnCraftCompleted?.Invoke(bandage.itemID, 1);

        return true;
    }

    private bool TrySpendUnitGold(IUnit_YHJ unit, int amount)
    {
        if (amount <= 0)
            return true;

        bool hasEventResult = false;
        bool spendResult = false;

        EventBus_YHJ.RequestSpendUnitGold?.Invoke
        (
            unit,
            amount,
            result =>
            {
                hasEventResult = true;
                spendResult = result;
            }
        );

        if (!hasEventResult && unit is Component component)
        {
            var wallet = component.GetComponent<IUnitGoldWallet_YHJ>();
            if (wallet != null)
            {
                hasEventResult = true;
                spendResult = wallet.TrySpendGold(amount);
            }
        }

        if (!hasEventResult)
        {
            Debug.LogWarning("[Heal] 헌터 골드 지불 처리자가 없음");
        }

        EventBus_YHJ.OnSpendUnitGoldResult?.Invoke(unit, amount, spendResult);
        return hasEventResult && spendResult;
    }
}
