using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// BuildingButtonUI_YHJ
/// 
/// 역할: 건설/업그레이드 버튼의 UI를 표시하고, 버튼 클릭 시 실제 업그레이드를 실행합니다.
/// 
/// KJG 수정 내용:
/// - Awake()에서 BuildingLevelComponent_YHJ 자동 연결
/// - OnClickBuild() 메서드 추가 (Build 버튼 클릭 시 호출)
/// - 업그레이드 성공/실패에 따라 로그 출력
/// </summary>
public class BuildingButtonUI_YHJ : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text nameText;
    public Image icon;
    public Transform costArea;
    public GameObject costItemPrefab;
    public Image stateBar;
    public TMP_Text statusText;

    // ★ KJG 추가: 이 버튼이 관리하는 건물의 LevelComponent
    private BuildingLevelComponent_YHJ levelComponent;

    private void Awake()
    {
        levelComponent = GetComponentInParent<BuildingLevelComponent_YHJ>();
        if (levelComponent == null)
        {
            Debug.LogWarning("[BuildingButtonUI] BuildingLevelComponent_YHJ를 찾지 못했습니다.");
        }
    }

    public void Setup(string name, Sprite iconSprite, List<ReasourceCost_YHJ> costs, bool canBuild, bool alreadyBuilt)
    {
        nameText.text = name;
        icon.sprite = iconSprite;

        foreach (Transform child in costArea)
        {
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
            var btn = GetComponent<Button>();
            if (btn != null) btn.interactable = false;
            icon.color = Color.gray;
            costArea.gameObject.SetActive(false);
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

    // ★ KJG 추가: Build 버튼 클릭 이벤트 (Inspector에서 연결)
    public void OnClickBuild()
    {
        if (levelComponent == null)
        {
            Debug.LogError("[BuildingButtonUI] BuildingLevelComponent_YHJ를 찾을 수 없습니다.");
            return;
        }

        bool success = levelComponent.TryUpgrade();

        if (success)
        {
            Debug.Log($"[BuildingButtonUI] 업그레이드 성공!");
        }
        else
        {
            Debug.LogWarning("[BuildingButtonUI] 업그레이드 실패 (Gold 부족 또는 최대 레벨)");
        }
    }
}