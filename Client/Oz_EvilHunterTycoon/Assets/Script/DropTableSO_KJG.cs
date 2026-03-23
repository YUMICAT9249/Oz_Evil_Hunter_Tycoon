using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "Data/DropTable", order = 1)]
public class DropTableSO : ScriptableObject
{
    [System.Serializable]
    public class DropItem
    {
        public string itemId;       // 드롭 아이템 id
        [Range(0f, 100f)] public float probability;   // 확률 %
        public double minAmount;
        public double maxAmount;
        public bool isGuaranteed;   // 100% 드랍 강제 여부
    }

    [System.Serializable]
    public class DropGroup
    {
        public string groupId;      // Easy,Nomal,Hard 등
        public List<DropItem> drops = new List<DropItem>();
    }

    public List<DropGroup> dropGroups = new List<DropGroup>();
}
