using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    public TMP_Text text;
    public float speed = 2f;

    void Update()
    {
        float alpha = Mathf.PingPong(Time.time * speed, 1f);
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}