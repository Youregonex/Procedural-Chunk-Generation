using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Player
    public Vector2 playerPosition;
    public Inventory playerHotbar;
    public Inventory playerMainInventory;
    public float playerCurrentHealth;
    public float playerMaxHealth;

    // World Generation
    public List<ChunkSaveData> chunkSaveDataList = new List<ChunkSaveData>();

        // Chunk Settings
    public int chunkLayerCount;

        // Noise Settings
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float obstacleSpawnThreshold;
    public int seed;
    public Vector2 seamOffset;
    public Noise.NormalizeMode normalizeMode;

    // World items
    public List<ItemSaveData> itemSaveDataList = new List<ItemSaveData>();

    // World buildings

        // Buildings Save Data
    public List<BuildingSaveData> buildingSaveDataList = new List<BuildingSaveData>();

        // Containers Save Data
    public List<ContainerSaveData> containerSaveDataList = new List<ContainerSaveData>();
}
