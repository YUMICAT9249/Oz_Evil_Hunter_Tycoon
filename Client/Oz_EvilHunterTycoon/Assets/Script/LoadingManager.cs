using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    static string _nextScene;

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