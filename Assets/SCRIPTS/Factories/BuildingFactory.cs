using UnityEngine;

public class BuildingFactory
{
    public Building CreateBuilding(BuildingItemDataSO buildingItemDataSO)
    {
        Building building = GameObject.Instantiate(buildingItemDataSO.BuildingPrefab);
        building.Initialize(buildingItemDataSO);

        return building;
    }
}
