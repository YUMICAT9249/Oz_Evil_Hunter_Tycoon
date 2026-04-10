using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [KJG 실무 아키텍처] Bootstrapper_KJG - NullReference 완전 방지 + 상세 로그 버전
/// </summary>
public class Bootstrapper_KJG : MonoBehaviour
{
    [Header("부팅 후 이동할 Scene 이름")]
    [SerializeField] private string nextSceneName = "Ingame_Scene";

    private void Awake()
    {
        Debug.Log("🚀 [Bootstrapper_KJG] 게임 부팅 시작");

        DontDestroyOnLoad(gameObject);
        Debug.Log("✅ Bootstrapper 자신 DontDestroyOnLoad 완료");

        var locator = ServiceLocator_KJG.Instance;
        Debug.Log("✅ ServiceLocator_KJG 준비 완료");

        Debug.Log("📌 매니저 등록 시작...");

        RegisterManager<CurrencyManager_KJG>(locator);
        RegisterManager<SaveLoadManager_KJG>(locator);
        RegisterManager<EventManager_KJG>(locator);
        RegisterManager<AchievementManager_KJG>(locator);
        RegisterManager<DifficultyManager_KJG>(locator);
        RegisterManager<DataManager_KJG>(locator);
        RegisterManager<AudioManager_KJG>(locator);
        RegisterManager<GameManager_KJG>(locator);
        RegisterManager<MapManager_KJG>(locator);
        RegisterManager<HunterManager_PJS>(locator);
        RegisterManager<ExpManager_KJG>(locator);
        RegisterManager<BuildingManager_YHJ>(locator);
        RegisterManager<DropManager_KJG>(locator);
        RegisterManager<EffectManager_KJG>(locator);

        // LoadingManager (팀원 스크립트)
        LoadingManager loading = FindObjectOfType<LoadingManager>();
        if (loading != null)
        {
            locator.Register(loading);
            DontDestroyOnLoad(loading.gameObject);
            Debug.Log("✅ LoadingManager 등록 완료");
        }
        else
        {
            Debug.LogWarning("⚠️ LoadingManager를 Bootstrap Scene에서 찾을 수 없습니다!");
        }

        Debug.Log("✅ [Bootstrapper_KJG] 모든 매니저 등록 완료!");

        LoadingManager.LoadScene(nextSceneName);
    }

    private void RegisterManager<T>(ServiceLocator_KJG locator) where T : MonoBehaviour
    {
        T mgr = FindObjectOfType<T>();
        if (mgr != null)
        {
            locator.Register(mgr);
            DontDestroyOnLoad(mgr.gameObject);
            Debug.Log($"✅ {typeof(T).Name} 등록 + DontDestroyOnLoad 완료");
        }
        else
        {
            Debug.LogError($"❌ {typeof(T).Name}를 Bootstrap Scene에서 찾을 수 없습니다! (GameObject가 없거나 비활성화됨)");
        }
    }
}