using UnityEngine;

public class CameraMoveController_KSH : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 1f;

    private Camera _cam;
    private Vector3 _lastWorldPos;
    private bool _isDragging;

    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
#else
        HandleTouchDrag();
#endif
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _lastWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            _lastWorldPos.z = 0f;
        }
        else if (Input.GetMouseButton(0) && _isDragging)
        {
            Vector3 currentWorldPos = _cam.ScreenToWorldPoint(Input.mousePosition);
            currentWorldPos.z = 0f;

            Vector3 delta = _lastWorldPos - currentWorldPos;
            transform.position += delta * dragSpeed;

            _lastWorldPos = currentWorldPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }

    private void HandleTouchDrag()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            _isDragging = true;
            _lastWorldPos = _cam.ScreenToWorldPoint(touch.position);
            _lastWorldPos.z = 0f;
        }
        else if (touch.phase == TouchPhase.Moved && _isDragging)
        {
            Vector3 currentWorldPos = _cam.ScreenToWorldPoint(touch.position);
            currentWorldPos.z = 0f;

            Vector3 delta = _lastWorldPos - currentWorldPos;
            transform.position += delta * dragSpeed;

            _lastWorldPos = currentWorldPos;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            _isDragging = false;
        }
    }
}