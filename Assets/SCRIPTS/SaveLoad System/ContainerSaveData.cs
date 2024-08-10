using UnityEngine;

[System.Serializable]
public class ContainerSaveData : BuildingSaveData
{
    public Inventory containerInventory;

    public ContainerSaveData(BuildingItemDataSO buildingItemDataSO, Vector2 position, Inventory containerInventory) : base(buildingItemDataSO, position)
    {
        this.containerInventory = containerInventory;
    }
}