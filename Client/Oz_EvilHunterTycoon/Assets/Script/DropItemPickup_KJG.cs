using UnityEngine;

/// <summary>
/// DropItemPickup_KJG - 바닥에 떨어진 드랍 아이템에 붙이는 스크립트
/// 플레이어가 클릭하면 자동으로 수집됩니다. (원작 스타일)
/// </summary>
public class DropItemPickup_KJG : MonoBehaviour
{
    public DropItemType itemType { get; private set; }
    public int amount { get; private set; }

    private bool _isCollected = false;

    /// <summary>
    /// DropManager가 드랍 생성할 때 호출
    /// </summary>
    public void SetDropData(DropItemType type, int amt)
    {
        itemType = type;
        amount = amt;
    }

    private void OnMouseDown()   // 마우스 클릭으로 수집 (원작처럼 클릭 수집)
    {
        if (_isCollected) return;
        Collect();
    }

    public void Collect()
    {
        if (_isCollected) return;
        _isCollected = true;

        // 실제 수집 처리
        if (itemType == DropItemType.Gold)
        {
            Manager_KJG.Currency.AddGold(amount);
            Debug.Log($"[DropItemPickup_KJG] 골드 {amount} 수집");
        }
        else
        {
            Debug.Log($"[DropItemPickup_KJG] {itemType} {amount}개 수집");
            // 나중에 InventoryManager_KJG.AddItem(itemType, amount) 등으로 확장 가능
        }

        Destroy(gameObject);   // 드랍 아이템 제거
    }
}