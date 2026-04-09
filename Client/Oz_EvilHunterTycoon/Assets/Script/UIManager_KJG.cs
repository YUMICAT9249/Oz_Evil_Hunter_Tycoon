using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// UIManager_KJG
/// 
/// 역할:
/// - HP Bar를 **헌터/몬스터 아래**에 항상 표시
/// - 오브젝트 클릭 시 선택 UI 버튼 표시
/// </summary>
public class UIManager_KJG : BaseManager_KJG<UIManager_KJG>
{
    [Header("=== HP Bar 설정 ===")]
    [Tooltip("HP Bar Prefab (World Space Canvas 사용)")]
    [SerializeField] private GameObject hpBarPrefab;

    [Header("=== 선택 UI 설정 ===")]
    [Tooltip("클릭 시 뜨는 선택 UI Prefab")]
    [SerializeField] private GameObject selectionUIPrefab;

    [Header("=== 위치 조정 ===")]
    [Tooltip("HP Bar를 오브젝트 아래로 얼마나 띄울지 (Y축 음수 값)")]
    [SerializeField] private float hpBarYOffset = -1.8f;   // ← 아래로 띄우는 값 (조정 가능)

    // 오브젝트별 HP Bar 캐싱
    private readonly Dictionary<BaseWorldObject_KJG, HPBar_KJG> _hpBars
        = new Dictionary<BaseWorldObject_KJG, HPBar_KJG>();

    private void OnEnable()
    {
        Manager_KJG.Map.AddHealthChangedListener(HandleHealthChanged);
        Manager_KJG.Map.AddObjectClickedListener(HandleObjectClicked);
    }

    private void OnDisable()
    {
        if (Manager_KJG.Map != null)
        {
            Manager_KJG.Map.RemoveHealthChangedListener(HandleHealthChanged);
            Manager_KJG.Map.RemoveObjectClickedListener(HandleObjectClicked);
        }
    }

    private void HandleHealthChanged(BaseWorldObject_KJG obj, float currentHp, float maxHp)
    {
        if (obj == null) return;

        if (!_hpBars.ContainsKey(obj))
        {
            CreateHPBar(obj);
        }

        if (_hpBars.TryGetValue(obj, out HPBar_KJG hpBar))
        {
            hpBar.UpdateHP(currentHp, maxHp);
        }
    }

    private void HandleObjectClicked(BaseWorldObject_KJG clickedObject)
    {
        if (clickedObject == null || selectionUIPrefab == null) return;

        BuildingWorldObject_YHJ building = clickedObject as BuildingWorldObject_YHJ;
        if (building != null)
        {
            Debug.Log("건물 클릭");

            HandleBuildingUI(building);
            return;
        }

        Debug.Log($"[UIManager_KJG] {clickedObject.displayName} 클릭 → 선택 UI 표시");

        Instantiate(selectionUIPrefab,
                    clickedObject.transform.position + Vector3.up * 3f,
                    Quaternion.identity);
    }

    private void CreateHPBar(BaseWorldObject_KJG obj)
    {
        if (hpBarPrefab == null) return;

        // HP Bar를 오브젝트 **아래**에 생성
        Vector3 position = obj.transform.position + Vector3.up * hpBarYOffset;

        var hpBarGO = Instantiate(hpBarPrefab, position, Quaternion.identity);
        hpBarGO.transform.SetParent(obj.transform);   // 오브젝트 따라 움직임

        var hpBar = hpBarGO.GetComponent<HPBar_KJG>();
        if (hpBar != null)
            _hpBars[obj] = hpBar;
    }

    public void RemoveHPBar(BaseWorldObject_KJG obj)
    {
        if (_hpBars.TryGetValue(obj, out HPBar_KJG hpBar))
        {
            Destroy(hpBar.gameObject);
            _hpBars.Remove(obj);
        }
    }
    private void HandleBuildingUI(BuildingWorldObject_YHJ building)
    {
        var interactions = building.GetComponents<IBuildingInteraction_YHJ>();

        if (interactions == null || interactions.Length == 0)
        {
            Debug.Log("인터렉션 없음");
            return;
        }

        // 🔥 UI 생성
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