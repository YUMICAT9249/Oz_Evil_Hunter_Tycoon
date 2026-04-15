using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingPlacementManager_YHJ : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Collider2D villageBuildArea;

    [System.Serializable]
    public class BuildingData
    {
        public string name;
        public GameObject prefab;
        public Vector2Int size;
        public bool canOverlap;

        public List<ReasourceCost_YHJ> costs;

        public string buildingID;
        public bool isRoad;
        public bool canRotate = true;

        public BuildingType_YHJ buildingType;
        public BuildingLevelData_YHJ levelData;
    }

    [Header("Building List")]
    public BuildingData[] buildings;
    public int selectedIndex = -1; // ★ 자동으로 0번(마을회관) 선택되지 않게 수정

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject buildingButtonPrefab;
    [SerializeField] private GameObject buildPanel;

    private GameObject previewInstance;
    private List<SpriteRenderer> previewRenderers = new List<SpriteRenderer>();
    private Dictionary<Vector2Int, BuildingInstance_YHJ> gridMap = new Dictionary<Vector2Int, BuildingInstance_YHJ>();

    private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
    private HashSet<string> builtBuildingIDs = new HashSet<string>();
    private readonly List<GameObject> buildingButtonObjects = new List<GameObject>();
    private GameObject previewRoot;
    private GameObject previewUI;
    [SerializeField] private GameObject previewUIPrefab;
    private bool isPlacing = false;
    private bool canPlace = false;
    private bool isDragging = false;

    private Vector2Int currentGridPos;
    private Vector2Int buildingSize;

    Vector3 dragOffset;
    Vector3 mouseDownPos;
    float dragThreshold = 0.1f;

    public static BuildingPlacementManager_YHJ Instance { get; private set; }

    bool IsPointerOnPreview()
    {
        if (previewRoot == null) return false;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

        if (hit == null) return false;

        return hit.transform.IsChildOf(previewRoot.transform);
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResolveVillageBuildArea();
        RegisterPrePlacedBuildings();
        GenerateBuildingButtons();
        if (buildPanel != null)
            buildPanel.SetActive(false);
    }

    void ResolveVillageBuildArea()
    {
        if (villageBuildArea != null)
            return;

        if (HunterManager_PJS.Instance == null)
            return;

        BoxCollider2D[] areas = HunterManager_PJS.Instance.GetAllAreas();
        if (areas == null || areas.Length == 0)
            return;

        foreach (var area in areas)
        {
            if (area == null)
                continue;

            if (area.name.Contains("Village") || area.name.Contains("village") || area.name.Contains("마을"))
            {
                villageBuildArea = area;
                return;
            }
        }

        villageBuildArea = areas[0];
    }

    void OnDisable()
    {
        if (Instance == this)
            Instance = null;
    }

    void RegisterPrePlacedBuildings()
    {
        var worldObjects = FindObjectsOfType<BuildingWorldObject_YHJ>();

        foreach (var obj in worldObjects)
        {
            // ★ YHJ: Start 순서와 상관없이 초기 배치 건물 Instance/size를 먼저 보장해 점유 칸 누락 방지
            var instance = obj.EnsurePrePlacedInstance();

            if (instance == null)
            {
                Debug.LogWarning($"[초기건물] Instance 없음: {obj.name}");
                continue;
            }

            // ★ YHJ: 동적 건설은 GridToWorld + size offset 위치에 배치되므로,
            // 초기 배치 건물도 같은 기준으로 역산해야 점유칸이 오른쪽/아래로 밀리지 않음
            Vector3 originWorldPos = GetPrePlacedAnchorWorldPosition(obj) - GetGridOffset(instance.size);
            Vector2Int gridPos = WorldToGrid(originWorldPos);

            instance.origin = gridPos;
            instance.instance = obj.gameObject;

            // size 없으면 기본 1x1
            if (instance.size == Vector2Int.zero)
                instance.size = new Vector2Int(1, 1);

            // 셀 계산
            var cells = new List<Vector2Int>();

            for (int x = 0; x < instance.size.x; x++)
            {
                for (int y = 0; y < instance.size.y; y++)
                {
                    Vector2Int pos = new Vector2Int(gridPos.x + x, gridPos.y - y);
                    cells.Add(pos);

                    // gridMap 등록
                    gridMap[pos] = instance;

                    // occupied 등록
                    occupied.Add(pos);
                }
            }

            instance.occupiedCells = cells;

            // 단일 건물 제한
            if (!string.IsNullOrEmpty(instance.buildingID))
            {
                builtBuildingIDs.Add(instance.buildingID);
            }

            // 매니저 등록 (중요)
            instance.Register();

            Debug.Log($"[초기건물 등록 완료] {obj.name}");
        }
    }

    IEnumerator ApplySortingNextFrame(GameObject obj)
    {
        yield return null;

        var renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var r in renderers)
        {
            r.sortingLayerName = "Building";
            r.sortingOrder = 10;
        }
    }

    // ★ 좌하단 건설 버튼은 이 함수를 연결해서 사용
    public void ToggleBuildPanel()
    {
        if (buildPanel == null)
        {
            Debug.LogError("buildPanel 연결 안됨");
            return;
        }

        bool nextState = !buildPanel.activeSelf;
        buildPanel.SetActive(nextState);

        if (nextState)
        {
            buildPanel.transform.SetAsLastSibling();
        }
    }

    Vector3 GetPrePlacedAnchorWorldPosition(BuildingWorldObject_YHJ obj)
    {
        Transform visual = obj.transform.Find("Visual");
        if (visual != null)
        {
            float threshold = Mathf.Max(grid.cellSize.x, grid.cellSize.y) * 0.75f;
            if (Vector2.Distance(obj.transform.position, visual.position) > threshold)
            {
                // ★ YHJ: 일부 초기 배치 건물은 루트가 아니라 Visual 자식에 위치가 들어가므로 실제 배치 기준 위치를 보정
                return visual.position;
            }
        }

        return obj.transform.position;
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void HandleDragInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseDownPos.z = 0;

            if (IsPointerOnPreview())
            {
                isDragging = false;
            }
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 currentMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentMouse.z = 0;

            float dist = Vector3.Distance(mouseDownPos, currentMouse);

            if (!isDragging && dist > dragThreshold)
            {
                if (IsPointerOnPreview())
                {
                    isDragging = true;
                    dragOffset = previewRoot.transform.position - currentMouse;
                }
            }

            if (isDragging)
            {
                UpdatePreviewPosition();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    public bool ShouldBlockCameraDrag()
    {
        if (!isPlacing)
            return false;

        if (isDragging)
            return true;

        return IsPointerOnPreview();
    }

    void UpdatePreviewPosition()
    {
        if (previewRoot == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3 targetPos = mouseWorld + dragOffset;

        Vector2Int gridPos = WorldToGrid(targetPos);
        currentGridPos = gridPos;

        Vector3 worldPos = GridToWorld(gridPos);
        worldPos.z = -1f;

        previewRoot.transform.position = worldPos + GetGridOffset(buildingSize);

        canPlace = CanPlace(gridPos);

        foreach (var r in previewRenderers)
        {
            r.color = canPlace
                ? new Color(0, 1, 0, 0.5f)
                : new Color(1, 0, 0, 0.5f);
        }
    }

    public void RotatePreview()
    {
        if (selectedIndex < 0 || selectedIndex >= buildings.Length) return;

        var data = buildings[selectedIndex];

        if (!data.canRotate)
            return;

        foreach (var r in previewRenderers)
        {
            r.flipX = !r.flipX;
        }
    }

    void Update()
    {
        

        if (!isPlacing) return;

        if (IsPointerOverUI() && !isDragging) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        HandleDragInput();
    }

    List<Vector2Int> CalculateCells(Vector2Int startPos, Vector2Int size)
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                cells.Add(new Vector2Int(startPos.x + x, startPos.y - y));
            }
        }

        return cells;
    }

    void HandleClick()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorld);

        if (hits == null || hits.Length == 0) return;

        foreach (var hit in hits)
        {
            var btn = hit.GetComponent<ButtonWorld_YHJ>();

            if (btn == null)
                continue;

            switch (btn.buttonType)
            {
                case ButtonWorld_YHJ.ButtonType.Build:
                    OnClickBuild();
                    break;

                case ButtonWorld_YHJ.ButtonType.Cancel:
                    OnClickCancel();
                    break;

                case ButtonWorld_YHJ.ButtonType.Rotate:
                    RotatePreview();
                    break;
            }

            return;
        }
    }

    void GenerateBuildingButtons()
    {
        ClearBuildingButtons();

        foreach (var data in buildings)
        {
            GameObject obj = Instantiate(buildingButtonPrefab, content);
            buildingButtonObjects.Add(obj);

            var ui = obj.GetComponent<BuildingButtonUI_YHJ>();
            if (ui == null) continue;

            var sr = data.prefab.GetComponentInChildren<SpriteRenderer>(true);
            if (sr == null) continue;

            Sprite icon = sr.sprite;

            bool alreadyBuilt = builtBuildingIDs.Contains(data.buildingID);

            ui.Setup(data.name, icon, data.costs, true, alreadyBuilt);

            Button btn = obj.GetComponent<Button>();
            if (btn == null) continue;

            if (!data.isRoad && alreadyBuilt)
            {
                btn.interactable = false;
            }
            else
            {
                int index = System.Array.IndexOf(buildings, data);

                btn.onClick.AddListener(() =>
                {
                    SelectBuilding(index);
                    StartPlacement();

                    if (buildPanel != null)
                        buildPanel.SetActive(false);
                });
            }
        }
    }

    void ClearBuildingButtons()
    {
        foreach (var buttonObject in buildingButtonObjects)
        {
            if (buttonObject != null)
                Destroy(buttonObject);
        }

        buildingButtonObjects.Clear();
    }

    void SelectBuilding(int index)
    {
        if (index < 0 || index >= buildings.Length) return;

        selectedIndex = index;
    }

    public void StartPlacement()
    {
        // ★ 선택 안 했으면 시작 금지
        if (selectedIndex < 0 || selectedIndex >= buildings.Length)
        {
            Debug.LogWarning("건물 선택 안됨");
            return;
        }

        isPlacing = false;

        if (previewInstance != null)
            Destroy(previewInstance);

        if (previewUI != null)
            Destroy(previewUI);

        var data = buildings[selectedIndex];

        if (!data.isRoad && builtBuildingIDs.Contains(data.buildingID))
        {
            Debug.Log("이미 건설 완료된 건물");
            return;
        }

        if (previewRoot != null)
            Destroy(previewRoot);

        previewRenderers.Clear();
        previewRoot = new GameObject("PreviewRoot");

        previewInstance = Instantiate(data.prefab, previewRoot.transform);
        foreach (var worldObject in previewInstance.GetComponentsInChildren<BuildingWorldObject_YHJ>(true))
        {
            // ★ YHJ: 고스트 프리뷰는 실제 건물이 아니므로 초기 배치/맵 등록 로직을 막음
            worldObject.SetPreviewMode(true);
        }

        previewUI = Instantiate(previewUIPrefab, previewRoot.transform);

        var buttons = previewUI.GetComponentsInChildren<ButtonWorld_YHJ>();

        foreach (var btn in buttons)
        {
            if (btn.name == "BuildUIButton")
                btn.buttonType = ButtonWorld_YHJ.ButtonType.Build;
            else if (btn.name == "BuildCancelButton")
                btn.buttonType = ButtonWorld_YHJ.ButtonType.Cancel;
            else if (btn.name == "RotateBuildingButton")
                btn.buttonType = ButtonWorld_YHJ.ButtonType.Rotate;
        }

        var renderers = previewInstance.GetComponentsInChildren<SpriteRenderer>(true);

        var ui = previewUI.GetComponent<PreviewUI_YHJ>();
        ui.Setup(data.canRotate && !data.isRoad, this);

        if (renderers.Length == 0)
        {
            Debug.LogError("고스트 SpriteRenderer 없음: " + data.name);
            return;
        }

        foreach (var r in renderers)
        {
            r.sortingLayerID = SortingLayer.NameToID("Building");
            r.sortingOrder = 10;
            r.color = new Color(0, 1, 0, 0.5f);
            r.flipX = false;
            previewRenderers.Add(r);
        }

        GameObject hitObj = new GameObject("HitArea");
        hitObj.transform.SetParent(previewInstance.transform);
        hitObj.transform.localPosition = Vector3.zero;

        BoxCollider2D col = hitObj.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        buildingSize = data.size;

        float width = grid.cellSize.x * buildingSize.x;
        float baseHeight = grid.cellSize.y * buildingSize.y;
        float baseYOffset = (buildingSize.y - 1) * grid.cellSize.y * 0.5f;

        float extraYOffset = 0f;
        float heightMultiplier = 1f;

        if (buildingSize.y == 1)
        {
            extraYOffset = 0.2f;
            heightMultiplier = 1.6f;
        }
        else if (buildingSize.y == 2)
        {
            extraYOffset = 0.4f;
            heightMultiplier = 2.0f;
        }
        else if (buildingSize.y >= 3)
        {
            extraYOffset = 0.4f;
            heightMultiplier = 1.7f;
        }

        col.size = new Vector2(width, baseHeight * heightMultiplier);
        col.offset = new Vector2(0f, baseYOffset + extraYOffset);

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        float uiPadding = 0.3f;
        float colliderBottom = col.offset.y - (col.size.y * 0.5f);
        previewUI.transform.localPosition = new Vector3(0, colliderBottom - uiPadding, 0);

        isPlacing = true;
        UpdatePreviewPosition();
    }

    bool CanPlace(Vector2Int startPos)
    {
        if (selectedIndex < 0 || selectedIndex >= buildings.Length) return false;

        var data = buildings[selectedIndex];

        if (data.canOverlap)
            return true;

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int checkPos = new Vector2Int(startPos.x + x, startPos.y - y);

                if (!IsInsideVillageBuildArea(checkPos))
                    return false;

                if (occupied.Contains(checkPos))
                    return false;
            }
        }

        return true;
    }

    bool IsInsideVillageBuildArea(Vector2Int gridPos)
    {
        if (villageBuildArea == null)
            return true;

        Vector3 worldPos = GridToWorld(gridPos);
        return villageBuildArea.OverlapPoint(worldPos);
    }

    void TryPlace()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(0))
            return;

        if (!canPlace) return;
        if (selectedIndex < 0 || selectedIndex >= buildings.Length) return;

        Vector3 worldPos = GridToWorld(currentGridPos);
        worldPos.z = -1f;

        var data = buildings[selectedIndex];

        Vector3 finalPos = worldPos + GetGridOffset(buildingSize);
        GameObject obj = Instantiate(data.prefab, finalPos, Quaternion.identity);

        StartCoroutine(ApplySortingNextFrame(obj));

        // 셀 계산
        var cells = CalculateCells(currentGridPos, buildingSize);

        // Instance는 여기서 직접 생성하지 않는다
        BuildingInstance_YHJ instanceData = null;

        // 고유 ID 생성
        string uniqueBuildingID = System.Guid.NewGuid().ToString();

        // WorldObject 가져오기
        var worldObj = obj.GetComponent<BuildingWorldObject_YHJ>();
        if (worldObj != null)
        {
            // 여기서만 Instance 생성됨
            worldObj.Initialize(uniqueBuildingID, data.levelData);

            instanceData = worldObj.GetInstance();

            // 데이터 세팅
            instanceData.buildingType = data.buildingType;
            instanceData.origin = currentGridPos;
            instanceData.size = buildingSize;
            instanceData.instance = obj;
            instanceData.occupiedCells = cells;
            // ★ YHJ: 동적 건설 후 실제 건물 크기 기준으로 헌터 회피용 콜라이더를 다시 맞춤
            worldObj.RefreshObstacleCollider();
        }
        else
        {
            Debug.LogError($"[BuildingPlacement] {data.name}에 BuildingWorldObject_YHJ 없음");
            return;
        }

        // 인벤토리 연결
        var inventory = obj.GetComponent<BuildingInventory_YHJ>();
        if (inventory != null)
        {
            inventory.buildingInstance = instanceData;
        }
        else
        {
            Debug.LogWarning($"[Inventory] {data.name}에 BuildingInventory 없음");
        }

        // gridMap 등록
        foreach (var cell in cells)
        {
            gridMap[cell] = instanceData;
        }

        // 방향 적용
        bool flip = previewRenderers.Count > 0 && previewRenderers[0].flipX;

        var renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var r in renderers)
        {
            r.flipX = flip;
            r.sortingLayerID = SortingLayer.NameToID("Building");
            r.sortingOrder = 10;
        }

        // 점유 처리
        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(currentGridPos.x + x, currentGridPos.y - y);
                occupied.Add(pos);
            }
        }

        // 매니저 등록
        if (!data.isRoad && data.buildingType != BuildingType_YHJ.None)
        {
            instanceData.Register();
        }

        // 이름 설정
        worldObj.displayName = data.name;

        // 단일 건물 제한 처리
        if (!data.isRoad)
        {
            builtBuildingIDs.Add(data.buildingID);
            // ★ YHJ: 동적 건설 완료 후 빌드 패널 버튼을 건설됨/비활성화 상태로 즉시 갱신
            GenerateBuildingButtons();
            CancelPlacement();
        }

        Debug.Log("현재 gridMap 개수: " + gridMap.Count);
    }

    public void CancelPlacement()
    {
        isPlacing = false;

        if (previewRoot != null)
            Destroy(previewRoot);
    }

    public void OnClickBuild()
    {
        if (!isPlacing || !canPlace) return;

        TryPlace();
    }

    public void OnClickCancel()
    {
        CancelPlacement();
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = grid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector3Int cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        return grid.GetCellCenterWorld(cell);
    }

    Vector3 GetGridOffset()
    {
        if (selectedIndex < 0 || selectedIndex >= buildings.Length)
            return Vector3.zero;

        return GetGridOffset(buildings[selectedIndex].size);
    }

    Vector3 GetGridOffset(Vector2Int size)
    {
        float xOffset = 0.5f * (size.x - 1) * grid.cellSize.x;
        float yOffset = -0.5f * (size.y - 1) * grid.cellSize.y;
        return new Vector3(xOffset, yOffset, 0f);
    }
}
