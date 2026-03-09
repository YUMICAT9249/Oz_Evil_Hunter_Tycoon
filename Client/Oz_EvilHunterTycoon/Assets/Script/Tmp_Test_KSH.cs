using TMPro;
using UnityEngine;

public class Tmp_Test_KSH : MonoBehaviour
{
    public TMP_Text[] _textCase;

    void Start()
    {
        _textCase[0].text = "테스트 입니다";
        _textCase[1].text = "앞으로 잘 부탁드립니다.";
        _textCase[2].text = "네트워크가 너무 좋아";
        _textCase[3].text = "백엔드로 갈거에요~";
        _textCase[4].text = "과제는 싫습니다.~~~~~~~";
        _textCase[5].text = "백엔드 사랑, tcp사랑";
        

        for (int i = 0; i < 6; i++)
        {
            
            RectTransform rt = _textCase[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * 200, i*-160 + 940);
        }
    }
}
