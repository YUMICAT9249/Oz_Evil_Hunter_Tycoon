using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstrapper (게임 부팅 담당자)
/// 
/// 역할:
/// - 게임이 시작되자마자 제일 먼저 실행됨
/// - 모든 매니저를 ServiceLocator_KJG에 자동 등록
/// - 등록이 끝나면 다음 Scene(Title 또는 Main)으로 이동
/// 
/// 왜 필요한가?
/// - ServiceLocator에 매니저를 등록하지 않으면 Manager_KJG.Currency 등이 null이 되어 오류 발생
/// - 초기화 순서를 완벽하게 제어할 수 있음 (실무에서 매우 중요)
/// </summary>
public class Bootstrapper_KJG : MonoBehaviour
{
    [Header("부팅 후 이동할 Scene 이름")]
    [SerializeField] private string nextSceneName = "Title";   // ← 나중에 Title, MainMenu 등으로 변경 가능

    private void Awake()
    {
        Debug.Log("🚀 [Bootstrapper_KJG] 게임 부팅 시작 - 모든 매니저 등록 중...");

        // ServiceLocator에 모든 매니저 등록
        var locator = ServiceLocator_KJG.Instance;

        locator.Register(Manager_KJG.Currency);
        locator.Register(Manager_KJG.SaveLoad);
        locator.Register(Manager_KJG.Event);
        locator.Register(Manager_KJG.Achievement);
        locator.Register(Manager_KJG.Difficulty);
        locator.Register(Manager_KJG.Data);
        locator.Register(Manager_KJG.Audio);
        locator.Register(Manager_KJG.Game);

        Debug.Log("✅ [Bootstrapper_KJG] 모든 매니저 등록 완료! 다음 Scene으로 이동합니다.");

        // Bootstrap 완료 후 다음 Scene으로 이동
        SceneManager.LoadScene(nextSceneName);
    }
}