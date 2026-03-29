using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingButtonUI_YHJ : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text nameText;
    public Image icon;
    public Transform costArea;
    public GameObject costItemPrefab;
    public Image stateBar;
    public TMP_Text statusText;


    public void Setup(string name, Sprite iconSprite, List<ReasourceCost_YHJ> costs, bool canBuild, bool alreadyBuilt)
    {
        nameText.text = name;
        icon.sprite = iconSprite;
        
        foreach (Transform child in costArea)
        {
            if (costItemPrefab == null)
            {
                Debug.LogError("CostItemPrefab is NULL");
            }

            if (costArea == null)
            {
                Debug.LogError("CostArea is NULL");
            }

            Destroy(child.gameObject);
        }
        

        
        foreach (var cost in costs)
        {
            GameObject item = Instantiate(costItemPrefab, costArea);

            item.transform.Find("Icon").GetComponent<Image>().sprite = cost.icon;
            item.transform.Find("Text").GetComponent<TMP_Text>().text = cost.amount.ToString();
        }
        
        
        if (canBuild)
        {
            stateBar.color = Color.green;
            statusText.text = "건설";
        }
        
        else if (alreadyBuilt)
        {
            stateBar.color = Color.gray;
            statusText.text = "건설 완료"; 

        }

        else
        {
            stateBar.color = Color.red;
            statusText.text = "자원 부족";
        }

        Debug.Log("setup 실행" +name);
        Debug.Log("아이콘 스프라이트" + iconSprite);
        Debug.Log("아이콘 컴포넌트" + icon);
        Debug.Log("네임 텍스트" + nameText);
    }
}