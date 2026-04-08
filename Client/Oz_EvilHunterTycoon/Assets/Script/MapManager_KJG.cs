using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MapManager_KJG
/// 
/// 역할:
/// - 모든 오브젝트 중앙 관리
/// - HP 변경, 클릭 이벤트를 안전하게 외부에 제공
/// </summary>
public class MapManager_KJG : BaseManager_KJG<MapManager_KJG>
{
    private readonly List<BaseWorldObject_KJG> worldObjects = new List<BaseWorldObject_KJG>();

    // ==================== 내부 이벤트 ====================
    private event System.Action<BaseWorldObject_KJG, float, float> _onHealthChanged;
    private event System.Action<BaseWorldObject_KJG> _onObjectClicked;

    // ==================== 외부에서 구독/해제할 수 있는 메서드 ====================
    public void AddHealthChangedListener(System.Action<BaseWorldObject_KJG, float, float> listener)
    {
        _onHealthChanged += listener;
    }

    public void RemoveHealthChangedListener(System.Action<BaseWorldObject_KJG, float, float> listener)
    {
        _onHealthChanged -= listener;
    }

    public void AddObjectClickedListener(System.Action<BaseWorldObject_KJG> listener)
    {
        _onObjectClicked += listener;
    }

    public void RemoveObjectClickedListener(System.Action<BaseWorldObject_KJG> listener)
    {
        _onObjectClicked -= listener;
    }

    // ==================== 이벤트 발생 (BaseWorldObject_KJG에서 호출) ====================
    public void TriggerHealthChanged(BaseWorldObject_KJG obj, float currentHp, float maxHp)
    {
        _onHealthChanged?.Invoke(obj, currentHp, maxHp);
    }

    public void TriggerObjectClicked(BaseWorldObject_KJG clickedObject)
    {
        _onObjectClicked?.Invoke(clickedObject);
    }

    // ==================== 등록 / 제거 ====================
    public void RegisterObject(BaseWorldObject_KJG obj)
    {
        if (obj == null || worldObjects.Contains(obj)) return;
        worldObjects.Add(obj);
        Debug.Log($"[MapManager_KJG] 등록 완료 → {obj.displayName}");
    }

    public void UnregisterObject(BaseWorldObject_KJG obj)
    {
        if (obj == null) return;
        worldObjects.Remove(obj);
        Debug.Log($"[MapManager_KJG] 제거 완료 → {obj.displayName}");
    }

    public List<BaseWorldObject_KJG> GetAllWorldObjects() => new List<BaseWorldObject_KJG>(worldObjects);
}