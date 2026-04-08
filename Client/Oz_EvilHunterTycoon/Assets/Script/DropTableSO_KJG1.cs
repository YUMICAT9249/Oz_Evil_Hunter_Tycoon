using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "KJG/Drop Table", fileName = "New DropTable")]
public class DropTableSO_KJG : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        public DropItemType itemType;
        public int amount = 10;
        [Range(0f, 100f)] public float chance = 80f;
    }

    [Header("드랍 목록")]
    public List<DropEntry> drops = new List<DropEntry>();

    public List<DropEntry> GetDrops()
    {
        List<DropEntry> result = new List<DropEntry>();
        foreach (var entry in drops)
        {
            if (Random.value * 100f <= entry.chance)
                result.Add(entry);
        }
        return result;
    }
}

public enum DropItemType
{
    Gold,
    Material,
    RareMaterial,
    Essence
}