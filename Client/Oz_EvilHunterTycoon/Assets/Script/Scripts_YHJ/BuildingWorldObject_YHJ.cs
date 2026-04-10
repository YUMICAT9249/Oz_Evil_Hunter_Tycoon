using UnityEngine;

public class BuildingWorldObject_YHJ : BaseWorldObject_KJG
{
    private BuildingInstance_YHJ instance;

    [Header("건물 설정")]
    public BuildingType_YHJ buildingType;

    protected override void Awake()
    {
        base.Awake();

        maxHp = 0;
        OnHealthChanged(0, 0);

        displayName = gameObject.name;
    }

    public void Initialize(string buildingID, BuildingLevelData_YHJ levelData)
    {
        if (levelData == null)
        {
            Debug.LogError($"[BuildingWorldObject] LevelData 없음: {buildingID}");
        }

        instance = new BuildingInstance_YHJ();

        instance.buildingID = buildingID;
        instance.Initialize(buildingID, levelData, gameObject);
        instance.buildingType = buildingType;

        instance.Register();
    }

    public BuildingInstance_YHJ GetInstance()
    {
        return instance;
    }

    public override void OnClicked()
    {
        base.OnClicked();
        Debug.Log("건물 클릭됨: " + displayName);
    }

    public override void OnHealthChanged(float current, float max) { }
    public override void TakeDamage(float damage) { }
}