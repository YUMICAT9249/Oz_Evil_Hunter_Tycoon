using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaitingHunterUI_YHJ : MonoBehaviour
{
    public Transform listParent;
    public GameObject itemPrefab;

    private void OnEnable()
    {
        Debug.Log("[WaitingHunterUI_YHJ] OnEnable");
        Subscribe();
        RefreshUI();
    }

    private void Start()
    {
        Debug.Log("[WaitingHunterUI_YHJ] Start");
        Subscribe();
        RefreshUI();
    }

    private void OnDisable()
    {
        if (HunterManager_PJS.Instance != null)
        {
            HunterManager_PJS.Instance.OnWaitingListChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        Debug.Log("[WaitingHunterUI_YHJ] RefreshUI called");

        if (listParent == null)
        {
            Debug.LogWarning("[WaitingHunterUI_YHJ] listParent is null");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogWarning("[WaitingHunterUI_YHJ] itemPrefab is null");
            return;
        }

        if (HunterManager_PJS.Instance == null)
        {
            Debug.LogWarning("[WaitingHunterUI_YHJ] HunterManager_PJS.Instance is null");
            return;
        }

        var list = HunterManager_PJS.Instance.GetWaitingHunters();
        Debug.Log($"[WaitingHunterUI_YHJ] waiting hunters: {list.Count}");

        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var hunter in list)
        {
            if (hunter == null) continue;

            GameObject obj = Instantiate(itemPrefab, listParent);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            TMP_Text tmpText = obj.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = hunter.name;
                Debug.Log($"[WaitingHunterUI_YHJ] slot created: {hunter.name}");
                continue;
            }

            Text legacyText = obj.GetComponentInChildren<Text>();
            if (legacyText != null)
            {
                legacyText.text = hunter.name;
                Debug.Log($"[WaitingHunterUI_YHJ] slot created with legacy text: {hunter.name}");
                continue;
            }

            Debug.LogWarning("[WaitingHunterUI_YHJ] Created slot but no Text/TMP_Text found on prefab");
        }
    }

    private void Subscribe()
    {
        if (HunterManager_PJS.Instance == null)
        {
            Debug.LogWarning("[WaitingHunterUI_YHJ] Subscribe skipped - HunterManager_PJS.Instance is null");
            return;
        }

        HunterManager_PJS.Instance.OnWaitingListChanged -= RefreshUI;
        HunterManager_PJS.Instance.OnWaitingListChanged += RefreshUI;
        Debug.Log("[WaitingHunterUI_YHJ] Subscribed to OnWaitingListChanged");
    }
}
