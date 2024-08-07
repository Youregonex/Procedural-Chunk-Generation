using System.Collections.Generic;
using UnityEngine;

public class WorldBuildingSpawner : MonoBehaviour
{
    public static WorldBuildingSpawner Instance { get; private set; }

    [Header("Debug Fields")]
    [SerializeField] private List<Building> _buildingList = new List<Building>();

    private BuildingFactory _buildingFactory = new BuildingFactory();

    public List<Building> BuildingList => _buildingList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    private void OnDestroy()
    {
        foreach (Building building in _buildingList)
        {
            building.OnBuildingDestruction -= Building_OnBuildingDestruction;
        }
    }

    public Building CreateBuildingAtPosition(BuildingItemDataSO buildingItemDataSO, Vector2 position)
    {
        Building building = _buildingFactory.CreateBuildingAtPosition(buildingItemDataSO, position);
        _buildingList.Add(building);
        building.OnBuildingDestruction += Building_OnBuildingDestruction;
        return building;
    }

    private void Building_OnBuildingDestruction(Building building)
    {

        if(_buildingList.Contains(building))
        {
            building.OnBuildingDestruction -= Building_OnBuildingDestruction;
            _buildingList.Remove(building);
        }
        else
        {
            Debug.LogError($"Destroyed building wasn't tracked!");
        }
    }
}
