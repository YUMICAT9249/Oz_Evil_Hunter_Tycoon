using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 0.01f;
    public float dragThreshold = 10f;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    [Header("클릭 가능한 객체의 레이어 지정")]
    public LayerMask onClickLayer;

    private Vector2 startPos;
    private Vector2 lastPos;
    private bool isDragging = false;
    private bool hasDragged = false;

    private bool IsBlockedByBuildingPreview()
    {
        return BuildingPlacementManager_YHJ.Instance != null
            && BuildingPlacementManager_YHJ.Instance.ShouldBlockCameraDrag();
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        PC_Move();
#else
        Mobile_Move();
#endif
    }

    private void PC_Move()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsBlockedByBuildingPreview())
                return;

            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
                return;

            startPos = Input.mousePosition;
            lastPos = startPos;
            isDragging = true;
            hasDragged = false;
        }

        if (Input.GetMouseButton(0))
        {
            if (!isDragging) return;

            Vector2 currentPos = Input.mousePosition;
            Vector2 totalDelta = currentPos - startPos;

            if (totalDelta.magnitude > dragThreshold)
            {
                hasDragged = true;
                HandleDrag(currentPos);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!isDragging) return;

            if (!hasDragged)
                HandleClick(Input.mousePosition);

            isDragging = false;
        }
    }

    private void Mobile_Move()
    {
        if (Input.touchCount != 1)
        {
            isDragging = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (IsBlockedByBuildingPreview())
        {
            isDragging = false;
            return;
        }

        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startPos = touch.position;
                lastPos = startPos;
                isDragging = true;
                hasDragged = false;
                break;

            case TouchPhase.Moved:
                if (!isDragging) return;

                Vector2 totalDelta = touch.position - startPos;

                if (totalDelta.magnitude > dragThreshold)
                {
                    hasDragged = true;
                    HandleDrag(touch.position);
                }
                break;

            case TouchPhase.Ended:
                if (!isDragging) return;

                if (!hasDragged)
                    HandleClick(touch.position);

                isDragging = false;
                break;

            case TouchPhase.Canceled:
                isDragging = false;
                break;
        }
    }

    private void HandleDrag(Vector2 currentPos)
    {
        Vector2 delta = currentPos - lastPos;

        transform.position += new Vector3(
            -delta.x * dragSpeed,
            -delta.y * dragSpeed,
            0f
        );

        ClampCamera();
        lastPos = currentPos;
    }

    private void HandleClick(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, onClickLayer);

        if (hit.collider != null)
        {
            Debug.Log("클릭한 객체 정보 : " + hit.collider.name);

            
            OnClick_KSH obj = hit.collider.GetComponent<OnClick_KSH>();
            if (obj != null)
            obj.OnClick();
        }
    }

    private void ClampCamera()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
        pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
        transform.position = pos;
    }
}
