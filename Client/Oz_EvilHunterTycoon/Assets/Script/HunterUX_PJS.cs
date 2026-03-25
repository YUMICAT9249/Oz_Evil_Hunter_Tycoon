using TMPro;
using UnityEngine;

public class HunterUX_PJS : MonoBehaviour
{
    // 참조할 데이터 스크립트
    [SerializeField] private HunterData_PJS _data;
    // 헌터 머리위에 띄울 UI
    [SerializeField] private TextMeshProUGUI _nameText;

    // 데이터 할당 후 UI 업데이트하는 함수
    public void SetNameUI()
    {
        // 데이터 + 텍스트 오브젝트 연결 확인 후 실행
        if (_data != null && _nameText != null)
        {
            // 프로퍼티를 통해 가져온 이름을 UI 텍스트에 대입
            _nameText.text = _data.name;
        }
    }
}
