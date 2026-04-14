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


        if (alreadyBuilt)
        {
            stateBar.color = Color.gray;
            statusText.text = "건설됨";

            // 버튼도 비활성화
            var btn = GetComponent<Button>();
            if (btn != null)
                btn.interactable = false;

            // 아이콘도 회색 처리 (선택)
            icon.color = Color.gray;
            costArea.gameObject.SetActive(!alreadyBuilt);
        }
        else if (canBuild)
        {
            stateBar.color = Color.green;
            statusText.text = "건설";
        }
        else
        {
            stateBar.color = Color.red;
            statusText.text = "자원 부족";
        }
    }
}