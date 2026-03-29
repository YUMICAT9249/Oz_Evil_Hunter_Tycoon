using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleTouch : MonoBehaviour
{
    public void OnTouchStart()
    {
        LoadingManager.LoadScene("");
    }
}