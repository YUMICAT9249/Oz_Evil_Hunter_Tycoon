using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneController : MonoBehaviour
{
    public Slider progressBar;

    void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        string nextScene = LoadingManager.GetNextScene();

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            // 0~0.9까지 로딩입니다.
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (progressBar != null)
                progressBar.value = progress;

            // 로딩이 완료되면 지정 딜레이 만큼 멈추고 진입합니다.
            if (op.progress >= 0.9f && timer > 0.5f)
            {
                op.allowSceneActivation = true;
            }
        }
    }
}