using System.Collections.Generic;
using UnityEngine;

public class PreviewUI_YHJ : MonoBehaviour
{
    public Transform BuildUIButton;
    public Transform BuildCancelButton;
    public Transform RotateBuildingButton;

    private BuildingPlacementManager_YHJ manager;

    public void Setup(bool canRotate, BuildingPlacementManager_YHJ mgr)
    {
        manager = mgr;

        RotateBuildingButton.gameObject.SetActive(canRotate);

        ArrangeButtons(canRotate);
    }

    void ArrangeButtons(bool showRotate)
    {
        List<Transform> list = new List<Transform>();

        list.Add(BuildUIButton);

        list.Add(BuildCancelButton);

        if (showRotate)
            list.Add(RotateBuildingButton);

        float spacing = 0.5f;
        float startX = -spacing * (list.Count - 1) / 2f;

        for (int i = 0; i < list.Count; i++)
        {
            list[i].localPosition = new Vector3(startX + spacing * i, 0, 0);
        }
    }
}