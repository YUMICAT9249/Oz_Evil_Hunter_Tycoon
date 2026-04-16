using UnityEngine;

/// <summary>
/// DropItemPickup_KJG - 바닥에 떨어진 드랍 아이템
/// 클릭하면 자동 수집 + MaterialInventory로 전달
/// </summary>
public class DropItemPickup_KJG : MonoBehaviour
{
    public DropItemType itemType { get; private set; }
    public int amount { get; private set; }

    // ★ KJG 추가: 시각적으로 보여줄 SpriteRenderer
    private SpriteRenderer spriteRenderer;

    private bool _isCollected = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[DropItemPickup_KJG] SpriteRenderer 컴포넌트가 없습니다! Prefab에 추가해주세요.");
        }
    }

    /// <summary>
    /// DropManager에서 호출됨
    /// </summary>
    public void SetDropData(DropItemType type, int amt, Sprite iconSprite)
    {
        itemType = type;
        amount = amt;

        // ★ KJG 핵심 수정: 스프라이트 설정
        if (spriteRenderer != null && iconSprite != null)
        {
            spriteRenderer.sprite = iconSprite;
            spriteRenderer.sortingOrder = 10; // 다른 오브젝트 위에 보이게
        }
        else if (spriteRenderer != null)
        {
            Debug.LogWarning($"[DropItemPickup] {type} 스프라이트가 없습니다.");
        }
    }

    private void OnMouseDown()
    {
        if (_isCollected) return;
        Collect();
    }

    public void Collect()
    {
        if (_isCollected) return;
        _isCollected = true;

        if (Manager_KJG.Building != null)
        {
            bool success = Manager_KJG.Building.ConsumeMaterial(itemType, amount);
            if (success)
                Debug.Log($"[DropItemPickup] {itemType} {amount}개 소비 성공");
        }

        Destroy(gameObject);
    }
}