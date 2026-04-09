using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UIManager_KJG
/// 
/// 역할:
/// - HP Bar 표시
/// - 오브젝트 클릭 시 기본 UI 표시
/// - (추가) 건물 클릭 시 건물 UI 처리
/// </summary>
public class UIManager_KJG : BaseManager_KJG<UIManager_KJG>
{
    [Header("HP Bar 설정")]
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private float hpBarYOffset = -1.8f;

    [Header("선택 UI 설정")]
    [SerializeField] private GameObject selectionUIPrefab;

    private readonly Dictionary<BaseWorldObject_KJG, HPBar_KJG> _hpBars
        = new Dictionary<BaseWorldObject_KJG, HPBar_KJG>();

    protected override void Start()
    {
        base.Start();
        SubscribeToMapManager();
        Debug.Log("[UIManager_KJG] 초기화 완료");
    }

    private void SubscribeToMapManager()
    {
        if (Manager_KJG.Map == null) return;

        Manager_KJG.Map.AddHealthChangedListener(HandleHealthChanged);
        Manager_KJG.Map.AddObjectClickedListener(HandleObjectClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Manager_KJG.Map != null)
        {
            Manager_KJG.Map.RemoveHealthChangedListener(HandleHealthChanged);
            Manager_KJG.Map.RemoveObjectClickedListener(HandleObjectClicked);
        }
    }

    // ====================== HP Bar ======================
    private void HandleHealthChanged(BaseWorldObject_KJG obj, float currentHp, float maxHp)
    {
        if (obj == null || obj is BuildingWorldObject_YHJ) return;

        if (!_hpBars.TryGetValue(obj, out var hpBar))
            CreateHPBar(obj);

        if (_hpBars.TryGetValue(obj, out hpBar))
            hpBar.UpdateHP(currentHp, maxHp);
    }

    private void CreateHPBar(BaseWorldObject_KJG obj)
    {
        if (hpBarPrefab == null) return;

        Vector3 pos = obj.transform.position + Vector3.up * hpBarYOffset;
        var hpBarGO = Instantiate(hpBarPrefab, pos, Quaternion.identity);
        hpBarGO.transform.SetParent(obj.transform);

        var hpBar = hpBarGO.GetComponent<HPBar_KJG>();
        if (hpBar != null)
            _hpBars[obj] = hpBar;
    }

    // ====================== 클릭 처리 ======================
    private void HandleObjectClicked(BaseWorldObject_KJG clickedObject)
    {
        if (clickedObject == null) return;

        Debug.Log($"[UIManager_KJG] {clickedObject.displayName} 클릭");

        // 🔥 건물 먼저 처리 (너 파트)
        BuildingWorldObject_YHJ building = clickedObject as BuildingWorldObject_YHJ;
        if (building != null)
        {
            HandleBuildingUI(building);
            return;
        }

        // 🔹 기본 UI
        if (selectionUIPrefab != null)
        {
            Vector3 uiPos = clickedObject.transform.position + Vector3.up * 3f;
            Instantiate(selectionUIPrefab, uiPos, Quaternion.identity);
        }
    }

    public void RemoveHPBar(BaseWorldObject_KJG obj)
    {
        if (_hpBars.TryGetValue(obj, out var hpBar))
        {
            Destroy(hpBar.gameObject);
            _hpBars.Remove(obj);
        }
    }

    // ====================== 건물 UI ======================
    private void HandleBuildingUI(BuildingWorldObject_YHJ building)
    {
        var interactions = building.GetComponents<IBuildingInteraction_YHJ>();

        if (interactions == null || interactions.Length == 0)
        {
            Debug.Log("인터렉션 없음");
            return;
        }

        GameObject ui = Instantiate(selectionUIPrefab,
            building.transform.position + Vector3.up * 3f,
            Quaternion.identity);

        Debug.Log($"인터렉션 개수: {interactions.Length}");

        foreach (var interaction in interactions)
        {
            string name = interaction.GetType().Name;
            Debug.Log($"UI 생성 대상: {name}");
        }
    }
}