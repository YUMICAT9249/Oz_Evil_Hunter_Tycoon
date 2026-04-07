using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// [KJG 실무 아키텍처] MapManager_KJG
/// 
/// 역할:
/// - BaseWorldObject_KJG를 상속받은 모든 오브젝트를 중앙에서 관리
/// - HP Bar는 항상 표시 (OnHealthChanged 이벤트로 UIManager가 처리)
/// - 클릭하면 추가 선택 UI 버튼이 나타나도록 이벤트 발생
/// </summary>
public class MapManager_KJG : BaseManager_KJG<MapManager_KJG>
{
    private readonly List<BaseWorldObject_KJG> worldObjects = new List<BaseWorldObject_KJG>();

    // ==================== 등록 / 제거 ====================
    public void RegisterObject(BaseWorldObject_KJG obj)
    {
        if (obj == null || worldObjects.Contains(obj)) return;
        worldObjects.Add(obj);
        Debug.Log($"[MapManager_KJG] 등록 완료 → {obj.displayName} ({obj.GetType().Name})");
    }

    public void UnregisterObject(BaseWorldObject_KJG obj)
    {
        if (obj == null) return;
        worldObjects.Remove(obj);
        Debug.Log($"[MapManager_KJG] 제거 완료 → {obj.displayName}");
    }

    // ==================== 조회 ====================
    public List<BaseWorldObject_KJG> GetAllWorldObjects() => new List<BaseWorldObject_KJG>(worldObjects);

    // ==================== 이벤트 ====================
    public void OnObjectClicked(BaseWorldObject_KJG clickedObject)
    {
        if (clickedObject == null) return;
        Debug.Log($"[MapManager_KJG] 클릭됨 → {clickedObject.displayName}");
        // UIManager_KJG가 이 이벤트를 받아 추가 선택 UI 버튼을 띄웁니다.
    }

    public void OnHealthChanged(BaseWorldObject_KJG obj, float currentHp, float maxHp)
    {
        Debug.Log($"[MapManager_KJG] HP 변경 → {obj.displayName} : {currentHp}/{maxHp}");
        // UIManager_KJG가 HP Bar를 실시간으로 업데이트합니다.
    }
}