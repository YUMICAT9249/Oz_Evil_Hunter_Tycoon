using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UserCameraMove_YHJ : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Optional Bounds")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    // 카메라 관련 코드라인 김성호 추가함
    public static UserCameraMove_YHJ Instance { get; set; }

    public Camera camera; // 카메라 객체
    public bool IsTarget = false; // 카메라 타겟 여부

    public float zoomSize = 3f;
    public float zoomSpeed = 3f; // 카메라 줌아웃 속도
    public HunterController_PJS targetHunter; // 타겟 헌터

    private void Awake()
    {
        Instance = this;
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (IsTarget)
        {
            CameraChaseHunter();
        }
        else
        {
            HandleMove();
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomSize, zoomSpeed * Time.deltaTime);
        }
            
    }

    private void HandleMove()
    {
        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        Vector3 moveDir = new Vector3(h, v, 0f).normalized;
        Vector3 nextPos = transform.position + moveDir * moveSpeed * Time.deltaTime;

        if (useBounds)
        {
            nextPos.x = Mathf.Clamp(nextPos.x, minPosition.x, maxPosition.x);
            nextPos.y = Mathf.Clamp(nextPos.y, minPosition.y, maxPosition.y);
        }

        transform.position = new Vector3(nextPos.x, nextPos.y, transform.position.z);
    }

    public void TargetHunter(HunterController_PJS _targetHunter) // 헌터 객체 매개 변수
    {
        // 헌터 클릭 시 카메라 추적
        if (targetHunter != null) { return; }

        IsTarget = true;
        targetHunter = _targetHunter;
        camera.orthographicSize = 1.5f;


        Debug.Log("헌터 추적 시작");
    }

    public void NoTarget()
    {
        // 추적 모드 종료
        IsTarget = false;
        targetHunter = null;
    }

    private void CameraChaseHunter()
    {
        if (targetHunter == null) { return; }

        transform.position = new Vector3(targetHunter.transform.position.x, targetHunter.transform.position.y, -10);
    }
}