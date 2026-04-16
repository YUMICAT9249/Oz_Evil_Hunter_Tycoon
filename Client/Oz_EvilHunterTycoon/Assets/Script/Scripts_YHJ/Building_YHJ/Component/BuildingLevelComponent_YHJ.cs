using UnityEngine;

/// <summary>
/// BuildingLevelComponent_YHJ
///
/// 역할:
/// - 건물의 현재 레벨 정보와 스탯을 관리합니다.
/// - 업그레이드 가능 여부 판단, 실제 업그레이드 실행을 담당합니다.
///
/// 중요 변경점 (KJG가 수정한 부분):
/// - Gold 소비를 CurrencyManager를 통해 중앙에서 처리하도록 변경
/// - ref int gold 방식 제거 (CurrencyManager가 Gold를 관리하도록 일관성 확보)
/// - OnUpgraded 이벤트 추가 (업그레이드 성공 시 UI, Sound, SaveLoad 등이 알 수 있게)
/// </summary>
public class BuildingLevelComponent_YHJ : MonoBehaviour
{
    public BuildingInstance_YHJ instance;

    // ★ 레벨 변경 시 UI나 다른 시스템이 구독할 수 있는 이벤트
    public System.Action<int> OnLevelChanged;

    // ★ 현재 레벨의 스탯이 변경되었을 때 알리는 이벤트
    public System.Action<LevelStat> OnLevelStatChanged;

    // ★ KJG 추가: 업그레이드가 성공했을 때 발생하는 이벤트
    // 예: UI 새로고침, 사운드 재생, 저장 요청 등에서 사용 가능
    public event System.Action OnUpgraded;

    // 현재 레벨 (읽기 전용)
    public int CurrentLevel => IsValid() ? instance.currentLevel : 0;

    // 최대 레벨 (읽기 전용)
    public int MaxLevel => IsValid() ? instance.levelData.MaxLevel : 0;

    // 현재 레벨의 스탯 정보 (읽기 전용)
    public LevelStat CurrentStat => IsValid() ? instance.CurrentStat : null;

    void Start()
    {
        if (!IsValid()) return;

        // 시작할 때 이벤트 발생 (UI 초기화용)
        OnLevelChanged?.Invoke(CurrentLevel);
        OnLevelStatChanged?.Invoke(CurrentStat);
    }

    /// <summary>
    /// 업그레이드 가능한지 확인 (Gold 충분한지 체크)
    /// </summary>
    public bool CanUpgrade()
    {
        if (instance == null || instance.levelData == null) return false;
        if (CurrentLevel >= MaxLevel) return false;

        var nextStat = instance.levelData.levelStats[CurrentLevel];

        // ★ CurrencyManager를 통해 현재 Gold 확인
        return Manager_KJG.Currency.Gold >= nextStat.upgradeCost;
    }

    /// <summary>
    /// 실제 업그레이드 실행
    /// CurrencyManager를 통해 Gold를 소비하고, 성공 시 이벤트 발생
    /// </summary>
    public bool TryUpgrade()
    {
        if (instance == null) return false;

        bool result = instance.TryUpgrade();

        if (result)
        {
            // 레벨과 스탯이 바뀌었음을 알림
            OnLevelChanged?.Invoke(CurrentLevel);
            OnLevelStatChanged?.Invoke(CurrentStat);

            // 업그레이드 성공을 다른 시스템에게 알림
            OnUpgraded?.Invoke();

            // ★ KJG 추가: 업그레이드 성공 시 자동 저장
            if (Manager_KJG.SaveLoad != null)
                Manager_KJG.SaveLoad.GameSave();
        }
        return result;
    }

    /// <summary>
    /// 현재 레벨에서 특정 아이템을 사용할 수 있는지 확인
    /// </summary>
    public bool CanUseItem(string itemID)
    {
        if (!IsValid() || CurrentStat == null) return true;
        if (CurrentStat.unlockItemIDs == null || CurrentStat.unlockItemIDs.Count == 0) return true;
        return CurrentStat.unlockItemIDs.Contains(itemID);
    }

    /// <summary>
    /// 강제로 레벨 정보를 새로고침하고 싶을 때 사용
    /// </summary>
    public void RefreshLevelState()
    {
        if (!IsValid()) return;
        OnLevelChanged?.Invoke(CurrentLevel);
        OnLevelStatChanged?.Invoke(CurrentStat);
    }

    private bool IsValid()
    {
        return instance != null && instance.levelData != null;
    }
}