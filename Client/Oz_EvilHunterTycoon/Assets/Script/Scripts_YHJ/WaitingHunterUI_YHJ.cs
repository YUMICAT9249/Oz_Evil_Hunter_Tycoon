using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingHunterUI_YHJ : MonoBehaviour
{
    public Transform listParent;     // HunterList
    public GameObject itemPrefab;    // 버튼 프리팹

    void OnEnable()
    {
        Invoke(nameof(RefreshUI), 0.2f);
    }

    public void RefreshUI()
    {
        if (HunterManager_PJS.Instance == null)
        {
            Debug.LogError("HunterManager Instance 없음");
            return;
        }

        if (listParent == null)
        {
            Debug.LogError("listParent 없음");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("itemPrefab 없음");
            return;
        }

        var list = HunterManager_PJS.Instance.GetWaitingHunters();

        if (list == null)
        {
            Debug.LogError("대기 헌터 리스트 없음");
            return;
        }

        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var hunter in list)
        {
            if (hunter == null) continue;

            GameObject obj = Instantiate(itemPrefab, listParent);

            Text txt = obj.GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = hunter.name;
            }
        }
    }
}