using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [KJG] UIManager_KJG - Phase 1 Core UI 중앙 관리자 (팀원 UiManager 연동 버전)
/// </summary>
public class UIManager_KJG : BaseManager_KJG<UIManager_KJG>
{
    [Header("=== HP Bar 설정 ===")]
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private float hpBarYOffset = -1.8f;

    [Header("=== 선택 UI 설정 ===")]
    [SerializeField] private GameObject selectionUIPrefab;

    private readonly Dictionary<BaseWorldObject_KJG, HPBar_KJG> _hpBars = new Dictionary<BaseWorldObject_KJG, HPBar_KJG>();

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

    private void HandleHealthChanged(BaseWorldObject_KJG obj, float currentHp, float maxHp)
    {
        if (obj == null || obj is BuildingWorldObject_YHJ) return;

        if (!_hpBars.ContainsKey(obj))
            CreateHPBar(obj);

        if (_hpBars.TryGetValue(obj, out HPBar_KJG hpBar))
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

    private void HandleObjectClicked(BaseWorldObject_KJG clickedObject)
    {
        if (clickedObject == null) return;

        Debug.Log($"[UIManager_KJG] {clickedObject.displayName} 클릭");

        // 1. 건물 클릭 처리
        if (clickedObject is BuildingWorldObject_YHJ building)
        {
            HandleBuildingClicked(building);
            return;
        }

        // 2. Hunter 클릭 처리 (팀원 UiManager.TargetHunter와 정확히 맞춤)
        if (clickedObject is HunterController_PJS hunterController)
        {
            UiManager.Instance.TargetHunter(true, hunterController);
            return;
        }

        // 3. 일반 오브젝트 클릭 → 선택 UI 표시
        if (selectionUIPrefab != null)
        {
            Vector3 uiPos = clickedObject.transform.position + Vector3.up * 3f;
            Instantiate(selectionUIPrefab, uiPos, Quaternion.identity);
        }
    }

    private void HandleBuildingClicked(BuildingWorldObject_YHJ building)
    {
        Debug.Log($"[UIManager_KJG] 건물 클릭: {building.displayName}");
        // 추후 Building UI Prefab Instantiate 로직 추가 예정
    }

    public void RemoveHPBar(BaseWorldObject_KJG obj)
    {
        if (_hpBars.TryGetValue(obj, out HPBar_KJG hpBar))
        {
            Destroy(hpBar.gameObject);
            _hpBars.Remove(obj);
        }
    }
}