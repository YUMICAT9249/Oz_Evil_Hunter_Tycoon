using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingHunterUI_YHJ : MonoBehaviour
{
    public Transform listParent;
    public GameObject itemPrefab;

    void OnEnable()
    {
        if (HunterManager_PJS.Instance != null)
        {
            HunterManager_PJS.Instance.OnWaitingListChanged += RefreshUI;
        }

        RefreshUI();
    }

    void OnDisable()
    {
        if (HunterManager_PJS.Instance != null)
        {
            HunterManager_PJS.Instance.OnWaitingListChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        if (HunterManager_PJS.Instance == null) return;

        var list = HunterManager_PJS.Instance.GetWaitingHunters();

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