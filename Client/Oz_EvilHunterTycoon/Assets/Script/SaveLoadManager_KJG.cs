using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// SaveLoadManager_KJG - 최종 보강 버전
///
/// - Building currentLevel 완전 복원
/// - MaterialInventory_YHJ (드랍 재료) 
/// - Audio 볼륨 저장/로드
/// - Achievement 저장/로드
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
        public List<MaterialSaveData> Materials = new List<MaterialSaveData>();   // 드랍 재료 저장
        public List<string> UnlockedAchievements = new List<string>();

        // Audio 볼륨
        public float MasterVolume = 1f;
        public float BGMVolume = 0.8f;
        public float SFXVolume = 1f;
    }

    [Serializable] public class HunterSaveData { public string hunterName; public HunterJop job; public int level = 1; public int exp = 0; public AreaType areaType = AreaType.Village; }
    [Serializable] public class BuildingSaveData { public string buildingID; public int currentLevel = 1; public Vector2Int gridPosition; }
    [Serializable] public class MaterialSaveData { public DropItemType itemType; public int amount = 0; }

    // ==================== 저장 ====================
    public void GameSave()
    {
        SaveData data = new SaveData();
        data.Gold = Manager_KJG.Currency.Gold;

        // Hunter 저장 (기존)
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

        // Building currentLevel 저장 (기존)
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

        // ★★★ MaterialInventory_YHJ (드랍 재료) 저장 - 원작 고증 핵심
        var materialInventory = FindObjectOfType<MaterialInventory_YHJ>();
        if (materialInventory != null)
        {
            data.Materials = materialInventory.GetAllMaterials();
        }

        // Audio 볼륨 저장
        if (Manager_KJG.Audio != null)
        {
            data.MasterVolume = Manager_KJG.Audio.masterVolume;
            data.BGMVolume = Manager_KJG.Audio.bgmVolume;
            data.SFXVolume = Manager_KJG.Audio.sfxVolume;
        }

        string json = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[SaveLoadManager_KJG] 저장 완료 (Building + MaterialInventory + Audio 포함)");
    }

    // ==================== 로드 ====================
    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("[SaveLoadManager_KJG] 저장된 데이터 없음 → 새 게임");
            NewGameSetup();
            return;
        }

        string json = PlayerPrefs.GetString(SAVE_KEY);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Manager_KJG.Currency.SetGold(data.Gold);

        // Building currentLevel 복원
        if (Manager_KJG.Building != null)
        {
            foreach (var saved in data.Buildings)
            {
                var target = Manager_KJG.Building.Buildings.Find(b => b.buildingID == saved.buildingID);
                if (target != null)
                {
                    target.currentLevel = saved.currentLevel;
                    Debug.Log($"[SaveLoad] {saved.buildingID} 레벨 {saved.currentLevel}로 복원");
                }
            }
        }

        // ★★★ MaterialInventory_YHJ (드랍 재료) 로드 - 원작 고증 핵심
        var materialInventory = FindObjectOfType<MaterialInventory_YHJ>();
        if (materialInventory != null)
        {
            materialInventory.LoadMaterials(data.Materials);
        }

        // Audio 볼륨 로드
        if (Manager_KJG.Audio != null)
        {
            Manager_KJG.Audio.SetMasterVolume(data.MasterVolume);
            Manager_KJG.Audio.SetBGMVolume(data.BGMVolume);
            Manager_KJG.Audio.SetSFXVolume(data.SFXVolume);
        }

        Debug.Log("[SaveLoadManager_KJG] 로드 완료 (모든 데이터 완전 복원)");
    }

    public void NewGameSetup()
    {
        Manager_KJG.Currency.SetGold(1000);
        Debug.Log("[SaveLoadManager_KJG] 새 게임 초기화 완료");
    }
}