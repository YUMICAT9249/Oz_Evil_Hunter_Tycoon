using TMPro;
using UnityEngine;

public class Tmp_Test_JBJ : MonoBehaviour
{
    public TMP_Text[] _textCase;

    void Start()
    {
        _textCase[0].text = "테스트입니다!";
        _textCase[1].text = "잘 부탁드립니다~";
        _textCase[2].text = "배우기 위한 열정!";
        _textCase[3].text = "의지충만!";
        _textCase[4].text = "이헌타 화이팅~!";
        _textCase[5].text = "7팀 화이팅";


        for (int i = 0; i < 6; i++)
        {

            RectTransform rt = _textCase[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(i * 200, i * 160 + -940);
        }
    }
}
