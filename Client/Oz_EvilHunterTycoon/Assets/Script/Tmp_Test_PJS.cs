using TMPro;
using UnityEngine;

public class Tmp_Test_PJS : MonoBehaviour
{
    public TMP_Text[] _textCase;

    void Start()
    {
        _textCase[0].text = "안녕하세요. 박정수 입니다";
        _textCase[1].text = "다음 주부터 시작되는 팀플 잘해봐요";
        _textCase[2].text = "게임도 재밌고 개발도 재밌어요";
        _textCase[3].text = "근데 누가 유니티 쉽다고 했어요?";
        _textCase[4].text = "그냥 죄다 어려운거 같아요";
        _textCase[5].text = "저는 빨리 이직하고 싶어요";

        for (int i = 0; i < 6; i++)
        {
            
            RectTransform rt = _textCase[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * 200, i*-160 + 940);
        }
    }
}
