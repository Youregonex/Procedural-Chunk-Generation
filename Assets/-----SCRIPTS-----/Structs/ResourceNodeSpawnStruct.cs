using UnityEngine;

[System.Serializable]
public struct ResourceNodeSpawnStruct
{
    public ResourceNode resourceNodePrefab;

    [Range(0f, 1f)]
    public float spawnChance;

    public int maxPerChunk;
    public int minPerChunk;
}
