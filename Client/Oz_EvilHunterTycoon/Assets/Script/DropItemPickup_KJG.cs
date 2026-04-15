using UnityEngine;

/// <summary>
/// DropItemPickup_KJG - 바닥에 떨어진 드랍 아이템
/// 수집 시 MaterialInventory에 실제로 추가
/// </summary>
public class DropItemPickup_KJG : MonoBehaviour
{
    public DropItemType itemType { get; private set; }
    public int amount { get; private set; }

    private bool _isCollected = false;

    public void SetDropData(DropItemType type, int amt)
    {
        itemType = type;
        amount = amt;
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

        // ★ 수집 성공 → MaterialInventory에 실제 추가
        if (MaterialInventory_YHJ.Instance != null)
        {
            string itemID = itemType.ToString();
            MaterialInventory_YHJ.Instance.AddItem(itemID, amount);
            Debug.Log($"[DropItemPickup] {itemType} {amount}개 수집 완료 → MaterialInventory 추가");
        }
        else
        {
            Debug.LogError("[DropItemPickup] MaterialInventory_YHJ.Instance가 없습니다!");
        }

        Destroy(gameObject);
    }
}