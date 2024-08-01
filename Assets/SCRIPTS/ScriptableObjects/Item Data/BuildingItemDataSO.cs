using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Data/Building Item Data")]
public class BuildingItemDataSO : ItemDataSO
{
    public event Action OnBuildingItemPlaced;

    [field: SerializeField] public Building BuildingPrefab { get; private set; }
    [field: SerializeField] public Vector2 BuildingSize { get; private set; }

    public void OnBuildingItemPlacedInvoke()
    {
        OnBuildingItemPlaced?.Invoke();
    }
}
