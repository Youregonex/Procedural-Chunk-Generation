using UnityEngine;

[System.Serializable]
public struct NodeResourceDropDataStruct
{
    public ItemDataSO dropResource;
    public int dropResourceAmountMin;
    public int dropResourceAmountMax;

    [Range(0f, 1f)]
    public float dropChance;
}
