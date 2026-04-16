using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// [KJG 실무 최고 수준] SaveLoadManager_KJG
/// 
/// - MaterialInventory (드랍 재료)
/// - Building currentLevel + capacity, workSpeed 등
/// - HunterData (exp, level, rebirth, rank 등)
/// - Currency (Gold, Cash, Exp)
/// - Audio 볼륨
/// - Achievement
/// </summary>
public class SaveLoadManager_KJG : BaseManager_KJG<SaveLoadManager_KJG>
{
    private const string SAVE_KEY = "EvilHunterTycoon_SaveData";

    [Serializable]
    public class SaveData
    {
        public double Gold = 0;
        public long Exp = 0;
        public int Cash = 0;
        public int DifficultyLevel = 1;

        public List<HunterSaveData> Hunters = new List<HunterSaveData>();
        public List<BuildingSaveData> Buildings = new List<BuildingSaveData>();
        public List<MaterialSaveData> Materials = new List<MaterialSaveData>();

        public List<string> UnlockedAchievements = new List<string>();

        // Audio
        public float MasterVolume = 1f;
        public float BGMVolume = 0.8f;
        public float SFXVolume = 1f;
    }

    [Serializable]
    public class HunterSaveData
    {
        public string hunterName;
        public HunterJop job;
        public int level = 1;
        public long exp = 0;                    // long으로 유지 (큰 숫자 대응)
        public AreaType areaType = AreaType.Village;
        public int rebirthCount = 0;
        public HunterRank rank = HunterRank.Normal;
    }

    [Serializable]
    public class BuildingSaveData
    {
        public string buildingID;
        public int currentLevel = 1;
        public Vector2Int gridPosition;
        public int capacity;
        public float workSpeed;
    }

    [Serializable]
    public class MaterialSaveData
    {
        public DropItemType itemType;
        public int amount = 0;
    }

    // ==================== 저장 ====================
    public void GameSave()
    {
        SaveData data = new SaveData();

        // Currency
        data.Gold = Manager_KJG.Currency.Gold;
        data.Exp = Manager_KJG.Currency.Exp;
        data.Cash = Manager_KJG.Currency.Cash;

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
                    exp = (long)hData._currentExp,           // ★ float → long 명시적 캐스트
                    areaType = hData._areaType,
                    rebirthCount = hData._rebirthCount,
                    rank = hData._hunterRank
                });
            }
        }

        // Building 저장
        if (Manager_KJG.Building != null)
        {
            foreach (var building in Manager_KJG.Building.Buildings)
            {
                if (building == null) continue;
                data.Buildings.Add(new BuildingSaveData
                {
                    buildingID = building.buildingID,
                    currentLevel = building.currentLevel,
                    gridPosition = building.origin,
                    capacity = building.capacity,
                    workSpeed = building.workSpeed
                });
            }
        }

        // MaterialInventory 저장
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

        Debug.Log("[SaveLoadManager_KJG] 저장 완료");
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

        // Currency 로드
        Manager_KJG.Currency.SetGold(data.Gold);
        Manager_KJG.Currency.SetExp(data.Exp);
        Manager_KJG.Currency.SetCash(data.Cash);

        // Building 로드
        if (Manager_KJG.Building != null)
        {
            foreach (var saved in data.Buildings)
            {
                var target = Manager_KJG.Building.Buildings.Find(b => b.buildingID == saved.buildingID);
                if (target != null)
                {
                    target.currentLevel = saved.currentLevel;
                    target.capacity = saved.capacity;
                    target.workSpeed = saved.workSpeed;
                }
            }
        }

        // MaterialInventory 로드
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

        Debug.Log("[SaveLoadManager_KJG] 로드 완료");
    }

    public void NewGameSetup()
    {
        Manager_KJG.Currency.SetGold(1000);
        Debug.Log("[SaveLoadManager_KJG] 새 게임 초기화 완료");
    }
}