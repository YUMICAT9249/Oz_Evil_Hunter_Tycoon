using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingPlacementManager_YHJ : MonoBehaviour
{
    [SerializeField] private Grid grid;

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
    public int selectedIndex = 0;

    [Header("UI")]
    [SerializeField] private Transform content;
    [SerializeField] private GameObject buildingButtonPrefab;
    [SerializeField] private GameObject buildPanel;

    private GameObject previewInstance;
    private List<SpriteRenderer> previewRenderers = new List<SpriteRenderer>();
    private Dictionary<Vector2Int, BuildingInstance_YHJ> gridMap = new Dictionary<Vector2Int, BuildingInstance_YHJ>();

    private HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();
    private HashSet<string> builtBuildingIDs = new HashSet<string>();
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

    void Start()
    {
        GenerateBuildingButtons();

        if (buildPanel != null)
            buildPanel.SetActive(false);
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

    void TryPlace()
    {
        if (EventSystem.current.IsPointerOverGameObject(0))
            return;

        if (!canPlace) return;

        Vector3 worldPos = GridToWorld(currentGridPos);
        worldPos.z = -1f;

        var data = buildings[selectedIndex];

        Vector3 finalPos = worldPos + GetGridOffset();
        GameObject obj = Instantiate(data.prefab, finalPos, Quaternion.identity);

        // Instance 생성
        BuildingInstance_YHJ instanceData = new BuildingInstance_YHJ
        {
            buildingID = data.buildingID,
            origin = currentGridPos,
            size = buildingSize,
            instance = obj,
            occupiedCells = CalculateCells(currentGridPos, buildingSize)
        };

        // 타입 연결
        instanceData.buildingType = data.buildingType;

        // 레벨 연결
        instanceData.levelData = data.levelData;

        // gridMap 등록
        foreach (var cell in instanceData.occupiedCells)
        {
            gridMap[cell] = instanceData;
        }

        // 점유 처리
        foreach (var pos in instanceData.occupiedCells)
        {
            occupied.Add(pos);
        }

        // 매니저 등록
        if (!data.isRoad && data.buildingType != BuildingType_YHJ.None)
        {
            instanceData.Register();
        }

        if (!data.isRoad)
        {
            builtBuildingIDs.Add(data.buildingID);
            CancelPlacement();
        }
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

    public void OnClickBuild()
    {
        if (!isPlacing || !canPlace) return;

        TryPlace();
    }

    public void OnClickCancel()
    {
        CancelPlacement();
    }

    public void CancelPlacement()
    {
        isPlacing = false;

        if (previewRoot != null)
            Destroy(previewRoot);
    }

    void GenerateBuildingButtons()
    {
        foreach (var data in buildings)
        {
            GameObject obj = Instantiate(buildingButtonPrefab, content);

            var ui = obj.GetComponent<BuildingButtonUI_YHJ>();
            if (ui == null) continue;

            var sr = data.prefab.GetComponentInChildren<SpriteRenderer>(true);
            if (sr == null) continue;

            Sprite icon = sr.sprite;

            bool alreadyBuilt = builtBuildingIDs.Contains(data.buildingID);

            ui.Setup(data.name, icon, data.costs, true, alreadyBuilt);

            Button btn = obj.GetComponent<Button>();
            if (btn == null) continue;

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

    void SelectBuilding(int index)
    {
        if (index < 0 || index >= buildings.Length) return;
        selectedIndex = index;
    }

    public void StartPlacement()
    {
        isPlacing = true;

        var data = buildings[selectedIndex];

        if (previewRoot != null)
            Destroy(previewRoot);

        previewRoot = new GameObject("PreviewRoot");

        previewInstance = Instantiate(data.prefab, previewRoot.transform);
        previewUI = Instantiate(previewUIPrefab, previewRoot.transform);

        previewRenderers.Clear();
        previewRenderers.AddRange(previewInstance.GetComponentsInChildren<SpriteRenderer>());

        buildingSize = data.size;

        UpdatePreviewPosition();
    }

    void HandleDragInput()
    {
        if (Input.GetMouseButton(0))
        {
            UpdatePreviewPosition();
        }
    }

    void UpdatePreviewPosition()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2Int gridPos = WorldToGrid(mouseWorld);
        currentGridPos = gridPos;

        previewRoot.transform.position = GridToWorld(gridPos);

        canPlace = true;
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

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3Int cell = grid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        return grid.GetCellCenterWorld(new Vector3Int(gridPos.x, gridPos.y, 0));
    }

    Vector3 GetGridOffset()
    {
        var data = buildings[selectedIndex];
        float xOffset = 0.5f * (data.size.x - 1) * grid.cellSize.x;
        float yOffset = -0.5f * (data.size.y - 1) * grid.cellSize.y;
        return new Vector3(xOffset, yOffset, 0f);
    }

    public void RotatePreview()
    {
        foreach (var r in previewRenderers)
        {
            r.flipX = !r.flipX;
        }
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}