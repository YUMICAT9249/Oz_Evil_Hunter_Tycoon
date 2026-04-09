using UnityEngine;

/// <summary>
/// Manager Facade (완전 최종 버전)
///
/// 사용법:
/// Manager_KJG.Audio.PlaySFX("monster_death");
/// Manager_KJG.Drop.DropFromMonster(...);
/// Manager_KJG.Loading.LoadScene("Ingame_Scene");
///
/// 특징:
/// - 모든 매니저를 Manager_KJG.XXX 형태로만 접근 (일관성)
/// - Lazy Initialization + ServiceLocator 사용
/// - 누락된 매니저 없이 완전 등록
/// </summary>
public static class Manager_KJG
{
    // 캐싱 필드 (모든 매니저)
    private static CurrencyManager_KJG _currency;
    private static SaveLoadManager_KJG _saveLoad;
    private static EventManager_KJG _event;
    private static AchievementManager_KJG _achievement;
    private static DifficultyManager_KJG _difficulty;
    private static DataManager_KJG _data;
    private static AudioManager_KJG _audio;
    private static GameManager_KJG _game;
    private static MapManager_KJG _map;
    private static HunterManager_PJS _hunter;
    private static ExpManager_KJG _exp;
    private static BuildingManager_YHJ _building;
    private static DropManager_KJG _drop;

    
    private static LoadingManager _loading;

    // ==================== 속성들 ====================
    public static CurrencyManager_KJG Currency => _currency ??= ServiceLocator_KJG.Instance.Get<CurrencyManager_KJG>();
    public static SaveLoadManager_KJG SaveLoad => _saveLoad ??= ServiceLocator_KJG.Instance.Get<SaveLoadManager_KJG>();
    public static EventManager_KJG Event => _event ??= ServiceLocator_KJG.Instance.Get<EventManager_KJG>();
    public static AchievementManager_KJG Achievement => _achievement ??= ServiceLocator_KJG.Instance.Get<AchievementManager_KJG>();
    public static DifficultyManager_KJG Difficulty => _difficulty ??= ServiceLocator_KJG.Instance.Get<DifficultyManager_KJG>();
    public static DataManager_KJG Data => _data ??= ServiceLocator_KJG.Instance.Get<DataManager_KJG>();
    public static AudioManager_KJG Audio => _audio ??= ServiceLocator_KJG.Instance.Get<AudioManager_KJG>();
    public static GameManager_KJG Game => _game ??= ServiceLocator_KJG.Instance.Get<GameManager_KJG>();
    public static MapManager_KJG Map => _map ??= ServiceLocator_KJG.Instance.Get<MapManager_KJG>();
    public static HunterManager_PJS Hunter => _hunter ??= ServiceLocator_KJG.Instance.Get<HunterManager_PJS>();
    public static ExpManager_KJG Exp => _exp ??= ServiceLocator_KJG.Instance.Get<ExpManager_KJG>();
    public static BuildingManager_YHJ Building => _building ??= ServiceLocator_KJG.Instance.Get<BuildingManager_YHJ>();
    public static DropManager_KJG Drop => _drop ??= ServiceLocator_KJG.Instance.Get<DropManager_KJG>();

    public static LoadingManager Loading => _loading ??= ServiceLocator_KJG.Instance.Get<LoadingManager>();
}