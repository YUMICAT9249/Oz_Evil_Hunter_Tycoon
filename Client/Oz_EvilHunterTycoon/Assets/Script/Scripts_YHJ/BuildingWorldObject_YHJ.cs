using UnityEngine;

public class BuildingWorldObject_YHJ : BaseWorldObject_KJG, OnClick_KSH
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

    public bool BlocksUnitMovement => !isPreview && buildingType != BuildingType_YHJ.None;

    protected override void Awake()
    {
        base.Awake();

        maxHp = 0;
        OnHealthChanged(0, 0);

        displayName = gameObject.name;

        EnsureObstacleCollider();
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

        EnsureObstacleCollider();

        // ★ YHJ: 동적 건설 건물도 자기 BuildingInstance를 알아야 레벨/업그레이드/재배치 데이터가 유지됨
        var levelComp = GetComponent<BuildingLevelComponent_YHJ>();
        if (levelComp != null)
        {
            levelComp.instance = instance;
            levelComp.RefreshLevelState();
        }

        instance.Register();
    }

    public BuildingInstance_YHJ GetInstance()
    {
        return instance;
    }

    public void SetPreviewMode(bool value)
    {
        // ★ YHJ: 건설/재배치 고스트는 실제 건물이 아니므로 Instance 생성과 Map 등록을 막음
        isPreview = value;
    }

    public BuildingInstance_YHJ EnsurePrePlacedInstance()
    {
        if (instance != null)
            return instance;

        if (prePlacedLevelData == null)
            return null;

        instance = new BuildingInstance_YHJ();
        instance.buildingID = prePlacedBuildingID;
        instance.Initialize(instance.buildingID, prePlacedLevelData, gameObject);
        instance.buildingType = buildingType;
        instance.size = prePlacedSize;

        EnsureObstacleCollider();

        // ★ YHJ: 초기 배치 건물도 레벨 컴포넌트가 자기 Instance를 알아야 레벨/점유칸/재배치 데이터가 유지됨
        var levelComp = GetComponent<BuildingLevelComponent_YHJ>();
        if (levelComp != null)
        {
            levelComp.instance = instance;
            levelComp.RefreshLevelState();
        }

        instance.Register();
        return instance;
    }

    public override void OnClicked()
    {
        base.OnClicked();
        Debug.Log("건물 클릭됨: " + displayName);
    }

    public void OnClick()
    {
        // ★ YHJ: KSH 카메라 클릭 Raycast 시스템이 건물도 기존 클릭 로직으로 연결되도록 어댑터 제공
        UiManager.Instance.BossUI();
        OnClicked();
    }

    public void RefreshObstacleCollider()
    {
        EnsureObstacleCollider();
    }

    private void EnsureObstacleCollider()
    {
        if (!BlocksUnitMovement)
            return;

        var boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider == null)
        {
            // ★ YHJ: 헌터/몬스터가 건물 위치를 장애물로 감지할 수 있도록 건물 루트에 감지용 콜라이더를 보장
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        boxCollider.isTrigger = true;

        Vector2Int size = instance != null && instance.size != Vector2Int.zero
            ? instance.size
            : prePlacedSize;

        if (size == Vector2Int.zero)
            size = Vector2Int.one;

        boxCollider.size = new Vector2(size.x, size.y);
        boxCollider.offset = new Vector2((size.x - 1) * 0.5f, -(size.y - 1) * 0.5f);
    }

    public override void OnHealthChanged(float current, float max) { }
    public override void TakeDamage(float damage) { }
    void Start()
    {
        if (isPreview)
            return;

        if (instance == null)
        {
            Debug.Log($"[초기건물 Instance 생성] {gameObject.name}");

            if (EnsurePrePlacedInstance() == null)
            {
                Debug.LogError("PrePlaced LevelData 없음");
                return;
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
