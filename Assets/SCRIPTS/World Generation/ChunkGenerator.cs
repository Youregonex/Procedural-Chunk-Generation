using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Tilemaps;
using Unity.Netcode;
using Youregone.Utilities;

public class ChunkGenerator : NetworkBehaviour//, IDataPersistance
{
    public static ChunkGenerator Instance { get; private set; }

    [Header("Tile Setup")]
    [SerializeField] private TileConfigSO _tileConfigSO;

    [Header("Chunk Setup")]
    [SerializeField] private Transform _chunkParent;
    [SerializeField] private Chunk _chunkPrefab;
    [SerializeField] private int _chunkLayerCount;

    [Header("Chunk Config")]
    [SerializeField] private NodeResourceSpawnConfigSO _nodeResourceSpawnConfigSO;

    [Header("Map Gentration Settings")]
    [SerializeField] private float _noiseScale = 10f;
    [SerializeField] private int _octaves = 10;
    [SerializeField, Range(0, 1)] private float _persistance = .5f;
    [SerializeField] private float _lacunarity = 1f;
    [SerializeField] private float _obstacleSpawnThreshold = .2f;
    [SerializeField] private int _seed;
    [SerializeField] private Vector2 _seamOffset = Vector2.zero;
    [SerializeField] private Noise.NormalizeMode _normalizeMode = Noise.NormalizeMode.Local;
    [SerializeField] private bool _useRandomSeed = true;

    [Header("Debug Fields")]
    [SerializeField] private bool _showGizmos = false;

    [SerializeField] private Dictionary<Vector2Int, Chunk> _chunkDictionary = new Dictionary<Vector2Int, Chunk>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

        if(_useRandomSeed)
            _seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    }

    public override void OnNetworkSpawn()
    {
        if(IsHost)
            StartGeneration();
    }

    public void StartGeneration()
    {
        CreateInitialChunk();
    }

    //public void SaveData(ref GameData gameData)
    //{
    //    if (!IsHost)
    //        return;

    //    gameData.chunkSaveDataList = new List<ChunkSaveData>();

    //    foreach (KeyValuePair<Vector2Int, Chunk> keyValuePair in _chunkDictionary)
    //    {
    //        gameData.chunkSaveDataList.Add(keyValuePair.Value.GenerateSaveData() as ChunkSaveData);
    //    }

    //    gameData.chunkLayerCount = _chunkLayerCount;

    //    gameData.noiseScale = _noiseScale;
    //    gameData.octaves = _octaves;
    //    gameData.persistance = _persistance;
    //    gameData.lacunarity = _lacunarity;
    //    gameData.obstacleSpawnThreshold = _obstacleSpawnThreshold;
    //    gameData.seed = _seed;
    //    gameData.seamOffset = _seamOffset;
    //    gameData.normalizeMode = _normalizeMode;
    //}

    //public void LoadData(GameData gameData)
    //{
    //    if (!IsHost)
    //        return;

    //    _chunkLayerCount = gameData.chunkLayerCount;

    //    _noiseScale = gameData.noiseScale;
    //    _octaves = gameData.octaves;
    //    _persistance = gameData.persistance;
    //    _lacunarity = gameData.lacunarity;
    //    _obstacleSpawnThreshold = gameData.obstacleSpawnThreshold;
    //    _seed = gameData.seed;
    //    _seamOffset = gameData.seamOffset;
    //    _normalizeMode = gameData.normalizeMode;

    //    for (int i = 0; i < gameData.chunkSaveDataList.Count; i++)
    //    {
    //        if (_chunkDictionary.ContainsKey(gameData.chunkSaveDataList[i].position))
    //        {
    //            Debug.Log("Chunk already exists!");
    //            continue;
    //        }

    //        ChunkSaveData chunkSaveData = gameData.chunkSaveDataList[i];

    //        Chunk chunk = GenerateChunk(chunkSaveData.position, true);
    //        chunk.LoadFromSaveData(chunkSaveData);

    //        if (chunk.ChunkDataFilled && chunk.IsAnyPlayerInRange)
    //            StartCoroutine(FillChunkData(chunk));
    //    }
    //}

    private void CreateInitialChunk()
    {
        if (IsHost)
            GenerateChunk(new Vector2Int(0, 0));
    }

    public void PlayerEnteredChunk(Chunk enteredChunk)
    {
        StartCoroutine(PlayerEnteredChunkCoroutine(enteredChunk));
    }

    private IEnumerator PlayerEnteredChunkCoroutine(Chunk enteredChunk)
    {
        if (!enteredChunk.ChunkDataFilled)
        {
            yield return StartCoroutine(FillChunkData(enteredChunk));
        }

        LoadChunk(enteredChunk);

        if (!IsHost)
            yield break;

        List<Vector2Int> enteredChunkNeighbourPositionList = enteredChunk.NeighbourChunkList;

        foreach (Vector2Int chunkNeighbourPosition in enteredChunkNeighbourPositionList)
        {
            if (!_chunkDictionary.ContainsKey(chunkNeighbourPosition))
            {
                GenerateChunk(chunkNeighbourPosition);
            }
        }
    }

    public void PlayerLeftChunk(Chunk exitChunk)
    {
        if (exitChunk.IsLoadingTiles.Value)
        {
            exitChunk.OnFinishTileLoading += Chunk_OnFinishTileLoading;
            return;
        }

        if(!exitChunk.IsAnyPlayerInRange)
            UnloadChunk(exitChunk);
    }

    private void LoadChunk(Chunk chunk) => chunk.LoadChunk();
    private void UnloadChunk(Chunk chunk) => chunk.UnloadChunk();

    private void Chunk_OnFinishTileLoading(Chunk chunk)
    {
        chunk.OnFinishTileLoading -= Chunk_OnFinishTileLoading;

        if (!chunk.IsAnyPlayerInRange)
            UnloadChunk(chunk);
    }

    private Chunk GenerateChunk(Vector2Int chunkCenter, bool loadingFromSaveFile = false)
    {
        if (_chunkDictionary.ContainsKey(chunkCenter))
        {
            UnityEngine.Debug.Log($"Chunk at {chunkCenter} is already exists!");
            return null;
        }

        Vector2 chunkPosition = new(chunkCenter.x, chunkCenter.y);

        Chunk chunk = Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);
        chunk.GetComponent<NetworkObject>().Spawn(true);
        chunk.transform.SetParent(_chunkParent);
        chunk.transform.gameObject.name = $"Chunk ({chunkCenter.x} | {chunkCenter.y})";
        chunk.InitializeChunk(_chunkLayerCount);
        chunk.GenerateChunkNoiseMap(_seed, _noiseScale, _octaves, _persistance, _lacunarity, chunkCenter + _seamOffset, _normalizeMode);

        _chunkDictionary.Add(chunkCenter, chunk);

        if (!loadingFromSaveFile)
        {
            GenerateNodePositionMap(chunk);
        }

        return chunk;
    }

    private void GenerateNodePositionMap(Chunk parentChunk)
    {
        List<Vector2Int> nodeValidPositions = GetNodeValidPositions(parentChunk);
        Dictionary<ResourceNode, int>  nodesToSpawnDictionary = GetNodesToSpawn(_nodeResourceSpawnConfigSO, out int nodesAmount);

        if (nodesAmount > nodeValidPositions.Count)
        {
            UnityEngine.Debug.LogError("Too much nodes for chunk!");
            return;
        }

        Dictionary<Vector2Int, ResourceNode> resourceNodeMapDictionary = new();

        foreach(KeyValuePair<ResourceNode, int> keyValuePair in nodesToSpawnDictionary)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                int randomNodePosition = UnityEngine.Random.Range(0, nodeValidPositions.Count);

                resourceNodeMapDictionary.Add(nodeValidPositions[randomNodePosition], keyValuePair.Key);
                nodeValidPositions.RemoveAt(randomNodePosition);
            }
        }

        parentChunk.FillResourceNodeMap(resourceNodeMapDictionary);
    }

    private Dictionary<ResourceNode, int> GetNodesToSpawn(NodeResourceSpawnConfigSO nodeResourceSpawnConfigSO, out int nodesAmount)
    {
        Dictionary<ResourceNode, int> nodesToSpawnDictionary = new();
        nodesAmount = 0;

        foreach (ResourceNodeSpawnStruct nodeSpawnStruct in nodeResourceSpawnConfigSO.NodeResourceSpawn)
        {
            if (UnityEngine.Random.Range(0f, 1f) > nodeSpawnStruct.spawnChance)
                continue;

            int nodeAmountForChunk = UnityEngine.Random.Range(nodeSpawnStruct.minPerChunk, nodeSpawnStruct.maxPerChunk + 1);

            nodesToSpawnDictionary.Add(nodeSpawnStruct.resourceNodePrefab, nodeAmountForChunk);
            nodesAmount += nodeAmountForChunk;
        }

        return nodesToSpawnDictionary;
    }

    private List<Vector2Int> GetNodeValidPositions(Chunk parentChunk)
    {
        Vector2Int chunkCenter = parentChunk.GetChunkPositionVector2Int();
        List<Vector2Int> validNodePositionsList = new List<Vector2Int>();

        int loopIndexX = 0;
        int loopIndexY = 0;

        for (int x = chunkCenter.x - _chunkLayerCount; x <= chunkCenter.x + _chunkLayerCount; x++)
        {
            for (int y = chunkCenter.y - _chunkLayerCount; y <= chunkCenter.y + _chunkLayerCount; y++)
            {
                float chunkNoiseMapValue = parentChunk.GetChunkNoiseMapValueWithXY(loopIndexX, loopIndexY);
                ETileType tileType = GetTileTypeWithNoise(chunkNoiseMapValue);

                if (tileType == ETileType.Ground)
                {
                    Vector2Int nodePlacementPosition = new Vector2Int(x, y);
                    validNodePositionsList.Add(nodePlacementPosition);
                }

                loopIndexY++;
            }

            loopIndexX++;
            loopIndexY = 0;
        }

        return validNodePositionsList;
    }

    private IEnumerator FillChunkData(Chunk chunk)
    {
        if (chunk.GeneratingTileData.Value || chunk.SpawningNodes.Value)
            yield break;

        yield return FillChunkTileDataCoroutine(chunk);
        yield return PlaceResourceNodesCoroutine(chunk);
    }

    private IEnumerator FillChunkTileDataCoroutine(Chunk parentChunk)
    {
        if (parentChunk.GeneratingTileData.Value)
            yield break;

        parentChunk.StartGeneratingTileData();

        List<TileData> tileDataMapList = new();
        Vector2Int chunkCenter = parentChunk.GetChunkPositionVector2Int();

        int loopIndexX = 0;
        int loopIndexY = 0;

        for (int x = chunkCenter.x - _chunkLayerCount; x <= chunkCenter.x + _chunkLayerCount; x++)
        {
            for (int y = chunkCenter.y - _chunkLayerCount; y <= chunkCenter.y + _chunkLayerCount; y++)
            {
                int chunkSideLength = _chunkLayerCount * 2 + 1;
                float[,] chunkNoiseMapValue = Utility.UnflattenArray(parentChunk.NoiseMapArray.Value, chunkSideLength, chunkSideLength);

                Vector2Int tilPlacementPosition = new(x, y);
                TileBase tile = GetRandomTile(chunkNoiseMapValue[x, y], out ETileType tileType);
                TileData tileData = new(tile, tilPlacementPosition, tileType);

                tileDataMapList.Add(tileData);
                loopIndexY++;
            }

            loopIndexX++;
            loopIndexY = 0;

            yield return new WaitForEndOfFrame();
        }

        parentChunk.FillChunkTileDataMap(tileDataMapList);
        parentChunk.FinishGeneratingTileData();
    }

    private IEnumerator PlaceResourceNodesCoroutine(Chunk parentChunk)
    {
        if (parentChunk.SpawningNodes.Value)
            yield break;

        parentChunk.StartSpawningNodes();

        Dictionary<Vector2Int, ResourceNode> nodeDictionary = new();

        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in parentChunk.NodePositionMapDictionary)
        {
            ResourceNode resourceNode = Instantiate(keyValuePair.Value, new Vector3(keyValuePair.Key.x, keyValuePair.Key.y, 0f), Quaternion.identity);
            nodeDictionary.Add(keyValuePair.Key, resourceNode);
            resourceNode.transform.SetParent(parentChunk.transform);

            yield return new WaitForEndOfFrame();
        }

        parentChunk.FillNodeDictionary(nodeDictionary);
        parentChunk.FinishSpawningNodes();
    }

    private ETileType GetTileTypeWithNoise(float tileNoise)
    {
        return tileNoise <= _obstacleSpawnThreshold ? ETileType.Obstacle : ETileType.Ground;
    }

    private TileBase GetRandomTile(float tileNoise, out ETileType tileType)
    {
        tileType = GetTileTypeWithNoise(tileNoise);

        if(tileType == ETileType.Obstacle)
        {
            return _tileConfigSO.ObstacleTiles[0];
        }

        if(tileType == ETileType.Ground)
        {
            float placeDefaultTileChance = UnityEngine.Random.Range(0f, 1f);
            int randomTile;

            if (placeDefaultTileChance <= .5f)
                randomTile = 0;
            else
                randomTile = UnityEngine.Random.Range(1, _tileConfigSO.GroundTiles.Count);

            return _tileConfigSO.GroundTiles[randomTile];
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        if (!_showGizmos || _chunkDictionary.Count == 0)
            return;

        Gizmos.color = Color.red;

        foreach(KeyValuePair<Vector2Int, Chunk> keyValuePair in _chunkDictionary)
        {
            Gizmos.DrawSphere((Vector2)keyValuePair.Key, .5f);
        }
    }
}