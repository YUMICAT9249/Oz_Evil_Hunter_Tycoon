using UnityEngine;

public class BuildingWorldObject_YHJ : BaseWorldObject_KJG
{
    [Header("초기 배치 건물용")]
    [SerializeField] private bool isPrePlaced = false;
    [SerializeField] private string prePlacedBuildingID;
    [SerializeField] private BuildingLevelData_YHJ prePlacedLevelData;
    [SerializeField] private Vector2Int prePlacedSize = Vector2Int.one;
    [SerializeField] private bool isPreview = false;

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
    void Start()
    {
        if (instance == null)
        {
            Debug.Log($"[초기건물 Instance 생성] {gameObject.name}");

            instance = new BuildingInstance_YHJ();

            var preData = GetComponent<BuildingWorldObject_YHJ>();

            var levelData = preData.prePlacedLevelData;

            if (levelData == null)
            {
                Debug.LogError("PrePlaced LevelData 없음");
                return;
            }

            instance.buildingID = preData.prePlacedBuildingID;
            instance.Initialize(instance.buildingID, levelData, gameObject);
            instance.buildingType = buildingType;

            instance.Register();

            // ⭐ 핵심 연결
            var levelComp = GetComponent<BuildingLevelComponent_YHJ>();
            if (levelComp != null)
            {
                levelComp.instance = instance;
            }
        }

        TryRegisterToMap();
    }

    void TryRegisterToMap()
    {
        if (ServiceLocator_KJG.Instance == null)
        {
            Debug.Log("ServiceLocator 아직 없음");
            return;
        }

        var map = Manager_KJG.Map;

        if (map == null)
        {
            Debug.Log("Map 아직 준비 안됨 → 스킵");
            return;
        }

        RegisterToMapManager();
    }
    private void OnDestroy()
    {
        if (isPreview)
            return;

        if (ServiceLocator_KJG.Instance == null)
            return;

        var map = Manager_KJG.Map;

        if (map == null)
            return;

        UnregisterFromMapManager();
    }
}