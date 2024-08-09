using UnityEngine;
using System;

public class Building : MonoBehaviour, IGenerateSaveData
{
    public event Action<Building> OnBuildingDestruction;

    [Header("Debug Fields")]
    [SerializeField] protected BuildingItemDataSO _buildingItemDataSO;


    public virtual void Initialize(BuildingItemDataSO buildingItemDataSO)
    {
        _buildingItemDataSO = buildingItemDataSO;
    }

    public virtual SaveData GenerateSaveData()
    {
        BuildingSaveData buildingSaveData = new BuildingSaveData(_buildingItemDataSO, transform.position);

        return buildingSaveData;
    }

    public virtual void LoadFromSaveData(SaveData saveData)
    {
        BuildingSaveData buildingSaveData = saveData as BuildingSaveData;

        transform.position = buildingSaveData.position;
        _buildingItemDataSO = buildingSaveData.buildingItemDataSO;
    }

    protected virtual void OnDestroy()
    {
        OnBuildingDestruction?.Invoke(this);
    }
}