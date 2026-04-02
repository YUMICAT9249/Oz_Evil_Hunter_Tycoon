using UnityEngine;

public class UserCameraMove_YHJ : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Optional Bounds")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    private void Update()
    {
        HandleMove();
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
}