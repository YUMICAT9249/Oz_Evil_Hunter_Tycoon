using UnityEngine;
using UnityEngine.UI;

public class HPBar_KJG : MonoBehaviour
{
    [SerializeField] private Image hpFillImage;   // HP Bar의 채워지는 부분 (Image 컴포넌트)

    public void UpdateHP(float currentHp, float maxHp)
    {
        if (hpFillImage != null)
        {
            hpFillImage.fillAmount = currentHp / maxHp;
        }
    }

    // HP Bar가 사라질 때 호출
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}