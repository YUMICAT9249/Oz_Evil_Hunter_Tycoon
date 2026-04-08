using UnityEngine;
using UnityEngine.UI;

public class ImageAnim : MonoBehaviour
{
    [Header("재생할 스프라이트 3개 넣기")]
    public Sprite[] frames;

    [Header("한 프레임당 시간")]
    public float frameTime = 0.2f;

    [Header("반복 여부")]
    public bool loop = true;

    private UnityEngine.UI.Image image;
    private int currentFrame;
    private float timer;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        if (frames == null || frames.Length == 0)
            return;

        image.sprite = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= frameTime)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                    currentFrame = 0;
                else
                {
                    currentFrame = frames.Length - 1;
                    enabled = false;
                    return;
                }
            }

            image.sprite = frames[currentFrame];
        }
    }
}