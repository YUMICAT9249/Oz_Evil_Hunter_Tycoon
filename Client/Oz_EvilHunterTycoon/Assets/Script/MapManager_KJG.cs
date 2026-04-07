using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// MapManager_KJG
/// 
/// 현재 BuildingModel 클래스가 없어서 임시로 주석 처리했습니다.
/// BuildingModel이 만들어지면 주석을 풀어주세요.
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

    public List<BaseWorldObject_KJG> GetAllMonsters()
    {
        return worldObjects.FindAll(obj => obj is Monster_JBJ);
    }

    public List<BaseWorldObject_KJG> GetAllHunters()
    {
        return worldObjects.FindAll(obj => obj is HunterController_PJS); // Hunter 스크립트 이름에 맞게 조정
    }

    // ==================== Building 관련은 아직 클래스 없으므로 주석 처리 ====================
    // public List<BaseWorldObject_KJG> GetAllBuildings()
    // {
    //     return worldObjects.FindAll(obj => obj is BuildingModel);
    // }

    // ==================== 이벤트 ====================
    public void OnObjectClicked(BaseWorldObject_KJG clickedObject)
    {
        if (clickedObject == null) return;
        Debug.Log($"[MapManager_KJG] 클릭됨 → {clickedObject.displayName}");
    }

    public void OnHealthChanged(BaseWorldObject_KJG obj, float currentHp, float maxHp)
    {
        Debug.Log($"[MapManager_KJG] HP 변경 → {obj.displayName} : {currentHp}/{maxHp}");
    }
}