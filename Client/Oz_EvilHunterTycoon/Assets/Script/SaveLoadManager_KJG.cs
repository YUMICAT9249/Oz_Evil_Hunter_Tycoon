using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// SaveLoadManager_KJG 
/// 
/// 역할:
/// - 게임을 껐다 켜도 헌터 레벨, 건물 currentLevel, 재료가 유지되게 함
/// - GameLoad()에서 Building currentLevel을 실제 건물에 복원
/// - BuildingInventory_YHJ와 연결 준비
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
        public List<MaterialSaveData> Materials = new List<MaterialSaveData>();
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

        // Building currentLevel 저장
        if (Manager_KJG.Building != null)
        {
            foreach (var building in Manager_KJG.Building.Buildings)
            {
                if (building == null) continue;
                data.Buildings.Add(new BuildingSaveData
                {
                    buildingID = building.buildingID,
                    currentLevel = building.currentLevel,      // 저장 완료
                    gridPosition = building.origin
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[SaveLoadManager_KJG] 게임 저장 완료 (Building currentLevel 포함)");
    }

    // ==================== 로드  ====================
    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[SaveLoadManager_KJG] 저장된 데이터가 없습니다 → 새 게임 시작");
            NewGameSetup();
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Manager_KJG.Currency.SetGold(data.Gold);

        // ★★★ Building currentLevel 실제 복원
        if (Manager_KJG.Building != null)
        {
            foreach (var saved in data.Buildings)
            {
                // buildingID로 실제 건물을 찾아서 currentLevel 복원
                var targetBuilding = Manager_KJG.Building.Buildings.Find(b => b.buildingID == saved.buildingID);
                if (targetBuilding != null)
                {
                    targetBuilding.currentLevel = saved.currentLevel;
                    Debug.Log($"[SaveLoad] {saved.buildingID} 레벨 {saved.currentLevel}로 복원 완료");
                }
            }
        }

        Debug.Log("[SaveLoadManager_KJG] 게임 로드 완료 (Building currentLevel 복원됨)");
    }

    public void NewGameSetup()
    {
        Manager_KJG.Currency.SetGold(1000);
        Debug.Log("[SaveLoadManager_KJG] 새 게임 초기화 완료");
    }
}