using System.Collections.Generic;
using UnityEngine;

public class BuildingInstance_YHJ
{
    public string buildingID;
    public Vector2Int origin;
    public Vector2Int size;

    public GameObject instance;

    public List<Vector2Int> occupiedCells;
}