using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [KJG 실무 아키텍처] UIManager_KJG
///
/// 역할:
/// - HP Bar를 헌터/몬스터 아래에 항상 표시 (원작처럼)
/// - 오브젝트 클릭 시 선택 UI Prefab만 띄움
/// - 실제 HunterClickUI나 Building UI를 띄우는 것은 UI 팀에게 요청
/// </summary>
public class UIManager_KJG : BaseManager_KJG<UIManager_KJG>
{
    [Header("HP Bar 설정")]
    [SerializeField] private GameObject hpBarPrefab;
    [SerializeField] private float hpBarYOffset = -1.8f;       // HP Bar를 오브젝트 아래로 띄우는 값

    [Header("선택 UI 설정")]
    [SerializeField] private GameObject selectionUIPrefab;     // 클릭하면 뜨는 기본 선택 UI

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
        if (obj == null || obj is BuildingWorldObject_YHJ) return; // 빌딩은 HP Bar 안 띄움

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

        Debug.Log($"[UIManager_KJG] {clickedObject.displayName} 클릭됨");

        // 기본 선택 UI Prefab만 띄움
        if (selectionUIPrefab != null)
        {
            Vector3 uiPos = clickedObject.transform.position + Vector3.up * 3f;
            Instantiate(selectionUIPrefab, uiPos, Quaternion.identity);
        }

        // 실제 HunterClickUI나 Building UI는 UI 팀이 처리하도록 요청할 예정
    }

    public void RemoveHPBar(BaseWorldObject_KJG obj)
    {
        if (_hpBars.TryGetValue(obj, out var hpBar))
        {
            Destroy(hpBar.gameObject);
            _hpBars.Remove(obj);
        }
    }
}