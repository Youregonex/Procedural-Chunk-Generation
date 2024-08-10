using UnityEngine;

[System.Serializable]
public class BuildingSaveData : SaveData
{
    public BuildingItemDataSO buildingItemDataSO;    
    public Vector2 position;

    public BuildingSaveData(BuildingItemDataSO buildingItemDataSO, Vector2 position)
    {
        this.buildingItemDataSO = buildingItemDataSO;
        this.position = position;
    }
}
