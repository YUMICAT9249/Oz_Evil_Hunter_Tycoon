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

        // Gold은 제외 (원작 고증)
        if (itemType == DropItemType.Material || itemType == DropItemType.RareMaterial || itemType == DropItemType.Essence)
        {
            // BuildingManager에 재료 소비 요청
            if (Manager_KJG.Building != null)
            {
                bool success = Manager_KJG.Building.ConsumeMaterial(itemType, amount);
                if (success)
                {
                    Debug.Log($"[DropItemPickup_KJG] {itemType} {amount}개 → Building 업그레이드 비용으로 소비");
                }
            }
        }

        Destroy(gameObject);
    }
}