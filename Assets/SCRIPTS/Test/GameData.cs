using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Player
    public Vector2 playerPosition;

    // World Generation
    public List<ChunkSaveData> chunkSaveDataList = new List<ChunkSaveData>();

        //Chunk Settings
    public int chunkLayerCount;

        //Noise Settings
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float obstacleSpawnThreshold;
    public int seed;
    public Vector2 seamOffset;
    public Noise.NormalizeMode normalizeMode;
}
