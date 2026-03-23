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

    private bool isPlacing = false;
    private bool canPlace = false;

    private Vector2Int currentGridPos;
    private Vector2Int buildingSize;

    private Quaternion currentRotation = Quaternion.identity;

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

    void Update()
    {
        if (!isPlacing) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        MovePreview();

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation *= Quaternion.Euler(0, 0, 90);
            previewInstance.transform.rotation = currentRotation;
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
        var data = buildings[selectedIndex];

        if (!data.isRoad && builtBuildingIDs.Contains(data.buildingID))
        {
            Debug.Log("이미 건설 완료된 건물");
            return;
        }

        if (previewInstance != null)
            Destroy(previewInstance);

        previewInstance = Instantiate(data.prefab);
        StartCoroutine(ApplySortingNextFrame(previewInstance));

        previewRenderers.Clear();

        var renderers = previewInstance.GetComponentsInChildren<SpriteRenderer>(true);

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

            previewRenderers.Add(r);
        }

        buildingSize = data.size;

        currentRotation = Quaternion.identity;
        previewInstance.transform.rotation = currentRotation;

        isPlacing = true;
    }

    void MovePreview()
    {
        if (previewInstance == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2Int gridPos = WorldToGrid(mouseWorld);
        currentGridPos = gridPos;

        Vector3 worldPos = GridToWorld(gridPos);
        worldPos.z = -1f;

        previewInstance.transform.position = worldPos;

        canPlace = CanPlace(gridPos);

        foreach (var r in previewRenderers)
        {
            r.color = canPlace
                ? new Color(0, 1, 0, 0.5f)
                : new Color(1, 0, 0, 0.5f);
        }
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
        if (!canPlace) return;

        Vector3 worldPos = GridToWorld(currentGridPos);
        worldPos.z = -1f;

        var data = buildings[selectedIndex];

        GameObject obj = Instantiate(data.prefab, worldPos, currentRotation);
        StartCoroutine(ApplySortingNextFrame(obj));

        var renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);

        foreach (var r in renderers)
        {
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

        if (previewInstance != null)
            Destroy(previewInstance);
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
}