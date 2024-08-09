using System.Collections.Generic;
using UnityEngine;

public class WorldBuildingSpawner : MonoBehaviour, IDataPersistance
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

    public void SaveData(ref GameData gameData)
    {
        gameData.buildingSaveDataList = new List<BuildingSaveData>();
        gameData.containerSaveDataList = new List<ContainerSaveData>();

        for (int i = 0; i < _buildingList.Count; i++)
        {
            if(_buildingList[i].TryGetComponent(out ContainerBuilding chest))
            {
                ContainerSaveData containerSaveData = chest.GenerateSaveData() as ContainerSaveData;
                gameData.containerSaveDataList.Add(containerSaveData);
            }
            else
            {
                BuildingSaveData buildingSaveData = _buildingList[i].GenerateSaveData() as BuildingSaveData;
                gameData.buildingSaveDataList.Add(buildingSaveData);
            }
        }
    }

    public void LoadData(GameData gameData)
    {
        _buildingList = new List<Building>();

        for (int i = 0; i < gameData.buildingSaveDataList.Count; i++)
        {
            BuildingSaveData buildingSaveData = gameData.buildingSaveDataList[i];
            BuildingItemDataSO buildingItemDataSO = buildingSaveData.buildingItemDataSO;
            Vector2 position = buildingSaveData.position;

            Building building = CreateBuildingAtPosition(buildingItemDataSO, position);

            building.LoadFromSaveData(buildingSaveData);
        }

        for (int i = 0; i < gameData.containerSaveDataList.Count; i++)
        {
            ContainerSaveData containerSaveData = gameData.containerSaveDataList[i];
            BuildingItemDataSO buildingItemDataSO = containerSaveData.buildingItemDataSO;
            Vector2 position = containerSaveData.position;

            ContainerBuilding container = CreateBuildingAtPosition(buildingItemDataSO, position) as ContainerBuilding;

            container.LoadFromSaveData(containerSaveData);
        }
    }

    public Building CreateBuildingAtPosition(BuildingItemDataSO buildingItemDataSO, Vector2 position)
    {
        Building building = _buildingFactory.CreateBuilding(buildingItemDataSO);
        building.transform.position = position;

        building.OnBuildingDestruction += Building_OnBuildingDestruction;

        _buildingList.Add(building);


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
