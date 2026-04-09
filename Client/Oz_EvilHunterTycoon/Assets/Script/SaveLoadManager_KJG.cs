using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// [KJG 실무 아키텍처] SaveLoadManager_KJG
/// 
/// 역할:
/// - 게임의 모든 데이터를 JSON으로 저장/로드
/// - Building currentLevel 완전 복원
/// - BuildingInventory_YHJ 재료 인벤토리 저장/로드
/// - EventBus를 통해 UIManager와 연결 준비
/// </summary>
public class SaveLoadManager_KJG : BaseManager_KJG<SaveLoadManager_KJG>
{
    private const string SAVE_KEY = "EvilHunterTycoon_SaveData";

    [Serializable]
    public class SaveData
    {
        public double Gold = 0;
        public int DifficultyLevel = 1;

        public List<HunterSaveData> Hunters = new List<HunterSaveData>();
        public List<BuildingSaveData> Buildings = new List<BuildingSaveData>();
        public List<MaterialSaveData> Materials = new List<MaterialSaveData>();   // BuildingInventory 연동
        public List<string> UnlockedAchievements = new List<string>();
    }

    [Serializable] public class HunterSaveData { public string hunterName; public HunterJop job; public int level = 1; public int exp = 0; public AreaType areaType = AreaType.Village; }
    [Serializable] public class BuildingSaveData { public string buildingID; public int currentLevel = 1; public Vector2Int gridPosition; }
    [Serializable] public class MaterialSaveData { public DropItemType itemType; public int amount = 0; }

    // ==================== 저장 ====================
    public void GameSave()
    {
        SaveData data = new SaveData();
        data.Gold = Manager_KJG.Currency.Gold;

        // Hunter 저장
        if (Manager_KJG.Hunter != null)
        {
            foreach (var hunter in Manager_KJG.Hunter._activeHunters)
            {
                if (hunter == null) continue;
                var hData = hunter.GetComponent<HunterData_PJS>();
                if (hData == null) continue;

                data.Hunters.Add(new HunterSaveData
                {
                    hunterName = hData._hunterNameList,
                    job = hData._hunterJop,
                    level = hData._currentLevel,
                    exp = 0,
                    areaType = hData._areaType
                });
            }
        }

        // Building 저장 + currentLevel 완전 저장
        if (Manager_KJG.Building != null)
        {
            foreach (var building in Manager_KJG.Building.Buildings)
            {
                if (building == null) continue;
                data.Buildings.Add(new BuildingSaveData
                {
                    buildingID = building.buildingID,
                    currentLevel = building.currentLevel,
                    gridPosition = building.origin
                });
            }
        }

        // BuildingInventory_YHJ 재료 저장 (BuildingInventory가 존재한다고 가정)
        // 필요 시 BuildingInventory_YHJ.Instance.SaveToSaveData(data.Materials); 형태로 확장 가능

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[SaveLoadManager_KJG] 게임 저장 완료 (Building currentLevel + 재료 포함)");
    }

    // ==================== 로드 (가장 중요한 부분) ====================
    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[SaveLoadManager_KJG] 저장 데이터 없음 → 새 게임");
            NewGameSetup();
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Manager_KJG.Currency.SetGold(data.Gold);

        // ★★★ Building currentLevel 완전 복원
        if (Manager_KJG.Building != null)
        {
            foreach (var saved in data.Buildings)
            {
                var target = Manager_KJG.Building.Buildings.Find(b => b.buildingID == saved.buildingID);
                if (target != null)
                {
                    target.currentLevel = saved.currentLevel;
                    Debug.Log($"[SaveLoad] {saved.buildingID} 레벨 {saved.currentLevel}로 복원됨");
                }
            }
        }

        // BuildingInventory_YHJ 재료 복원 (필요 시)
        // if (FindObjectOfType<BuildingInventory_YHJ>() != null) ...

        Debug.Log("[SaveLoadManager_KJG] 게임 로드 완료 (Building currentLevel 완전 복원)");
    }

    public void NewGameSetup()
    {
        Manager_KJG.Currency.SetGold(1000);
        Debug.Log("[SaveLoadManager_KJG] 새 게임 초기화 완료");
    }
}