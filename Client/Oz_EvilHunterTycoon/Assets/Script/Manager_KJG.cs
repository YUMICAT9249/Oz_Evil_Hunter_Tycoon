using UnityEngine;

/// <summary>
/// [KJG 실무 아키텍처] Manager_KJG
///
/// 사용법:
/// Manager_KJG.Currency.AddGold(100);
/// Manager_KJG.Hunter.HunterRandomSpawn();
/// Manager_KJG.Building.RegisterBuilding(...);
/// </summary>
public static class Manager_KJG
{
    // 캐싱 필드
    private static CurrencyManager_KJG _currency;
    private static SaveLoadManager_KJG _saveLoad;
    private static EventManager_KJG _event;
    private static AchievementManager_KJG _achievement;
    private static DifficultyManager_KJG _difficulty;
    private static DataManager_KJG _data;
    private static AudioManager_KJG _audio;
    private static GameManager_KJG _game;
    private static MapManager_KJG _map;

    // Hunter & Exp & Building 추가
    private static HunterManager_PJS _hunter;
    private static ExpManager_KJG _exp;
    private static BuildingManager_YHJ _building;

    // 속성들
    public static CurrencyManager_KJG Currency => _currency ??= ServiceLocator_KJG.Instance.Get<CurrencyManager_KJG>();
    public static SaveLoadManager_KJG SaveLoad => _saveLoad ??= ServiceLocator_KJG.Instance.Get<SaveLoadManager_KJG>();
    public static EventManager_KJG Event => _event ??= ServiceLocator_KJG.Instance.Get<EventManager_KJG>();
    public static AchievementManager_KJG Achievement => _achievement ??= ServiceLocator_KJG.Instance.Get<AchievementManager_KJG>();
    public static DifficultyManager_KJG Difficulty => _difficulty ??= ServiceLocator_KJG.Instance.Get<DifficultyManager_KJG>();
    public static DataManager_KJG Data => _data ??= ServiceLocator_KJG.Instance.Get<DataManager_KJG>();
    public static AudioManager_KJG Audio => _audio ??= ServiceLocator_KJG.Instance.Get<AudioManager_KJG>();
    public static GameManager_KJG Game => _game ??= ServiceLocator_KJG.Instance.Get<GameManager_KJG>();
    public static MapManager_KJG Map => _map ??= ServiceLocator_KJG.Instance.Get<MapManager_KJG>();

    // Hunter, Exp, Building
    public static HunterManager_PJS Hunter => _hunter ??= ServiceLocator_KJG.Instance.Get<HunterManager_PJS>();
    public static ExpManager_KJG Exp => _exp ??= ServiceLocator_KJG.Instance.Get<ExpManager_KJG>();
    public static BuildingManager_YHJ Building => _building ??= ServiceLocator_KJG.Instance.Get<BuildingManager_YHJ>();
}