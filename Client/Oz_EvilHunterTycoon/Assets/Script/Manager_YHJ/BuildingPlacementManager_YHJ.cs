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

    bool IsPointerOnPreview()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

        if (hit == null) return false;

        return hit.transform.IsChildOf(previewRoot.transform);
    }

    void Start()
    {
        GenerateBuildingButtons();

        if (buildPanel != null)
            buildPanel.SetActive(false);
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

    public void ToggleBuildPanel()
    {
        if (buildPanel == null)
        {
            Debug.LogError("buildPanel 연결 안됨");
            return;
        }

        buildPanel.SetActive(!buildPanel.activeSelf);
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

            // 일정 거리 이상 움직여야 드래그 시작
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

    void UpdatePreviewPosition()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector3 targetPos = mouseWorld + dragOffset;

        Vector2Int gridPos = WorldToGrid(targetPos);
        currentGridPos = gridPos;

        Vector3 worldPos = GridToWorld(gridPos);
        worldPos.z = -1f;

        previewRoot.transform.position = worldPos + GetGridOffset();

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

        // UI 클릭이면 무시
        if (IsPointerOverUI() && !isDragging) return;

        // 클릭 처리 추가
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        // 드래그는 유지
        HandleDragInput();
    }

    void HandleClick()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

        if (hit == null) return;

        // 버튼인지 확인
        var btn = hit.GetComponent<ButtonWorld_YHJ>();

        if (btn != null)
        {
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

    void SelectBuilding(int index)
    {
        if (index < 0 || index >= buildings.Length) return;

        selectedIndex = index;
    }

    public void StartPlacement()
    {
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

        // 고스트 생성
        previewInstance = Instantiate(data.prefab, previewRoot.transform);

        // UI 생성
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

        // 회전 버튼 ON/OFF
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
            r.flipX = false; // 초기화

            previewRenderers.Add(r);
        }

        GameObject hitObj = new GameObject("HitArea");
        hitObj.transform.SetParent(previewRoot.transform);
        hitObj.transform.localPosition = Vector3.zero;

        BoxCollider2D col = hitObj.AddComponent<BoxCollider2D>();
        col.isTrigger = true;

        Bounds bounds = renderers[0].bounds;

        foreach (var r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        float offsetY = bounds.min.y - previewRoot.transform.position.y - 0.3f;
        previewUI.transform.localPosition = new Vector3(0, offsetY, 0);

        col.size = bounds.size;
        col.offset = bounds.center - previewInstance.transform.position;

        buildingSize = data.size;
        isPlacing = true;
    }

    bool CanPlace(Vector2Int startPos)
    {
        var data = buildings[selectedIndex];

        if (data.canOverlap)
            return true;

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int checkPos = new Vector2Int(startPos.x + x, startPos.y + y);

                if (occupied.Contains(checkPos))
                    return false;
            }
        }

        return true;
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
        StartCoroutine(ApplySortingNextFrame(obj));

        bool flip = previewRenderers[0].flipX;

        var renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var r in renderers)
        {
            r.flipX = flip;
            r.sortingLayerID = SortingLayer.NameToID("Building");
            r.sortingOrder = 10;
        }

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(currentGridPos.x + x, currentGridPos.y + y);
                occupied.Add(pos);
            }
        }

        if (!data.isRoad)
        {
            builtBuildingIDs.Add(data.buildingID);
            CancelPlacement();
        }
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
        var data = buildings[selectedIndex];
        float yOffset = -0.5f * data.size.y;
        return new Vector3(0f, yOffset, 0f);
    }
}