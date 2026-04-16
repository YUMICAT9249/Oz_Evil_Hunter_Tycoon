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

        // ★ MaterialInventory에 실제 재료 추가
        if (MaterialInventory_YHJ.Instance != null)
        {
            string itemID = itemType.ToString();
            MaterialInventory_YHJ.Instance.AddItem(itemID, amount);
            Debug.Log($"[DropItemPickup] {itemType} {amount}개 수집 완료 → MaterialInventory 추가");

            // ★ KJG 추가: 재료 수집 성공 시 자동 저장 (Save/Load 연동)
            if (Manager_KJG.SaveLoad != null)
            {
                Manager_KJG.SaveLoad.GameSave();
            }

            // ★ KJG 추가: UI 새로고침 (EventManager_KJG 사용)
            if (Manager_KJG.Event != null)
            {
                Manager_KJG.Event.RefreshUI();
            }
        }
        else
        {
            Debug.LogError("[DropItemPickup] MaterialInventory_YHJ.Instance가 없습니다!");
        }

        Destroy(gameObject);
    }
}