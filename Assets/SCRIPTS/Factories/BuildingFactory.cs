using UnityEngine;

public class BuildingFactory
{
    public Building CreateBuildingAtPosition(BuildingItemDataSO buildingItemDataSO, Vector2 position)
    {
        Transform itemTransform = GameObject.Instantiate(buildingItemDataSO.BuildingPrefab, position, Quaternion.identity).transform;

        Building building = itemTransform.GetComponent<Building>();

        return building;
    }
}
