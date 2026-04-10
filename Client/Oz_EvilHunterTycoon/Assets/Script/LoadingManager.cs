using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [팀원용] LoadingManager - BaseManager_KJG 상속 버전
/// 이제 Manager_KJG.Loading으로 접근 가능 + DontDestroyOnLoad 자동 적용
/// </summary>
public class LoadingManager : BaseManager_KJG<LoadingManager>
{
    private static string _nextScene;

    protected override void Start()
    {
        base.Start();   // BaseManager_KJG 초기화 (DontDestroyOnLoad 등)
        Debug.Log("[LoadingManager] 초기화 완료");
    }

    public static void LoadScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene("Loading_Scene");
    }

    public static string GetNextScene()
    {
        return _nextScene;
    }
}