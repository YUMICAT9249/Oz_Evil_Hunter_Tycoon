using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [KJG] Bootstrapper_KJG - 원래 가장 안정적이었던 방식 + 클릭 대기 기능
/// </summary>
public class Bootstrapper_KJG : MonoBehaviour
{
    [Header("부팅 후 이동할 Scene 이름")]
    [SerializeField] private string nextSceneName = "Ingame_Scene";

    private bool hasClicked = false;

    private void Awake()
    {
        Debug.Log("🚀 [Bootstrapper_KJG] 게임 부팅 시작");

        DontDestroyOnLoad(gameObject);

        var locator = ServiceLocator_KJG.Instance;
        Debug.Log("✅ ServiceLocator_KJG 준비 완료");

        Debug.Log("📌 매니저 등록 시작...");

        // 원래 가장 안정적이었던 .Instance 방식으로 등록
        locator.Register(CurrencyManager_KJG.Instance);
        locator.Register(SaveLoadManager_KJG.Instance);
        locator.Register(EventManager_KJG.Instance);
        locator.Register(AchievementManager_KJG.Instance);
        locator.Register(DifficultyManager_KJG.Instance);
        locator.Register(DataManager_KJG.Instance);
        locator.Register(AudioManager_KJG.Instance);
        locator.Register(GameManager_KJG.Instance);
        locator.Register(MapManager_KJG.Instance);
        locator.Register(HunterManager_PJS.Instance);
        locator.Register(ExpManager_KJG.Instance);
        locator.Register(BuildingManager_YHJ.Instance);
        locator.Register(DropManager_KJG.Instance);
        locator.Register(EffectManager_KJG.Instance);

        LoadingManager loading = FindObjectOfType<LoadingManager>();
        if (loading != null)
        {
            locator.Register(loading);
            DontDestroyOnLoad(loading.gameObject);
        }

        Debug.Log("✅ [Bootstrapper_KJG] 모든 매니저 등록 완료! 클릭 대기 중...");
    }

    private void Update()
    {
        if (hasClicked) return;

        // 화면 클릭(마우스 또는 터치) 감지
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            hasClicked = true;
            Debug.Log("👆 화면 클릭 감지 → LoadingManager 호출");

            LoadingManager.LoadScene(nextSceneName);
        }
    }
}