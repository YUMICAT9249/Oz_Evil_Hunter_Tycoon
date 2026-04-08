using UnityEngine;

/// <summary>
///  Manager Facade (정적 접근 클래스)
/// 
/// 사용법:
/// Manager_KJG.Currency.AddGold(100);
/// Manager_KJG.SaveLoad.GameSave();
/// Manager_KJG.Event.RequestSave();
/// 
/// 특징:
/// - Manager. 만 치면 모든 매니저가 IntelliSense로 자동완성됨
/// - 내부에서 ServiceLocator를 한 번만 조회하고 캐싱 → 빠르고 안전함
/// - 실무에서 "아키텍처가 깔끔하다"는 평가를 받기 좋은 형태
/// </summary>
public static class Manager_KJG
{
    // 캐싱 필드 (한 번 조회한 매니저는 다시 조회하지 않음 → 성능 향상)
    private static CurrencyManager_KJG _currency;
    private static SaveLoadManager_KJG _saveLoad;
    private static EventManager_KJG _event;
    private static AchievementManager_KJG _achievement;
    private static DifficultyManager_KJG _difficulty;
    private static DataManager_KJG _data;
    private static AudioManager_KJG _audio;
    private static GameManager_KJG _game;
    private static MapManager_KJG _map;

    // 외부에서 접근하는 속성들 (Manager_KJG.Currency 형태)
    public static CurrencyManager_KJG Currency => _currency ??= ServiceLocator_KJG.Instance.Get<CurrencyManager_KJG>();
    public static SaveLoadManager_KJG SaveLoad => _saveLoad ??= ServiceLocator_KJG.Instance.Get<SaveLoadManager_KJG>();
    public static EventManager_KJG Event => _event ??= ServiceLocator_KJG.Instance.Get<EventManager_KJG>();
    public static AchievementManager_KJG Achievement => _achievement ??= ServiceLocator_KJG.Instance.Get<AchievementManager_KJG>();
    public static DifficultyManager_KJG Difficulty => _difficulty ??= ServiceLocator_KJG.Instance.Get<DifficultyManager_KJG>();
    public static DataManager_KJG Data => _data ??= ServiceLocator_KJG.Instance.Get<DataManager_KJG>();
    public static AudioManager_KJG Audio => _audio ??= ServiceLocator_KJG.Instance.Get<AudioManager_KJG>();
    public static GameManager_KJG Game => _game ??= ServiceLocator_KJG.Instance.Get<GameManager_KJG>();
    public static MapManager_KJG Map => _map ??= ServiceLocator_KJG.Instance.Get<MapManager_KJG>();

    // 팀원 스크립트 (HunterManager_PJS 완성되면 여기에 추가)
    // public static HunterManager_PJS     Hunter      => _hunter ??= ServiceLocator_KJG.Instance.Get<HunterManager_PJS>();
}