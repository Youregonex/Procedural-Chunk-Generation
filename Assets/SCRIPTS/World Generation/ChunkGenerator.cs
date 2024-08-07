using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour, IDataPersistance
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

    [Header("Debug Fields")]
    [SerializeField] private bool _showGizmos = false;
    [SerializeField] private List<Chunk> _loadingChunks = new List<Chunk>();

    [SerializeField] private SerializableDictionary<Vector2Int, Chunk> _chunkDictionary = new SerializableDictionary<Vector2Int, Chunk>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    private void Start()
    {
        Chunk.OnPlayerEnteredChunkRange += Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange += Chunk_OnPlayerLeftChunkRange;

        //CreateInitialChunk();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            CreateInitialChunk();
        }
    }

    private void OnDestroy()
    {
        Chunk.OnPlayerEnteredChunkRange -= Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange -= Chunk_OnPlayerLeftChunkRange;
    }

    public void SaveData(ref GameData gameData)
    {
        foreach (KeyValuePair<Vector2Int, Chunk> keyValuePair in _chunkDictionary)
        {
            gameData.chunkSaveDataList.Add(keyValuePair.Value.GenerateSaveData());
        }

        gameData.chunkLayerCount = _chunkLayerCount;

        gameData.noiseScale = _noiseScale;
        gameData.octaves = _octaves;
        gameData.persistance = _persistance;
        gameData.lacunarity = _lacunarity;
        gameData.obstacleSpawnThreshold = _obstacleSpawnThreshold;
        gameData.seed = _seed;
        gameData.seamOffset = _seamOffset;
        gameData.normalizeMode = _normalizeMode;
    }

    public void LoadData(GameData gameData)
    {
        for(int i = 0; i < gameData.chunkSaveDataList.Count; i++)
        {
            if (_chunkDictionary.ContainsKey(gameData.chunkSaveDataList[i].position))
            {
                Debug.Log("Chunk already exists!");
                continue;
            }

            ChunkSaveData chunkSaveData = gameData.chunkSaveDataList[i];

            Chunk chunk = GenerateChunk(chunkSaveData.position);
            chunk.LoadChunkFromSaveData(chunkSaveData);

            StartCoroutine(FillChunkCoroutine(chunk));

            if(!_chunkDictionary.ContainsKey(chunk.Position))
                _chunkDictionary.Add(chunk.Position, chunk);
        }

        _chunkLayerCount = gameData.chunkLayerCount;

        _noiseScale = gameData.noiseScale;
        _octaves = gameData.octaves;
        _persistance = gameData.persistance;
        _lacunarity = gameData.lacunarity;
        _obstacleSpawnThreshold = gameData.obstacleSpawnThreshold;
        _seed = gameData.seed;
        _seamOffset = gameData.seamOffset;
        _normalizeMode = gameData.normalizeMode;
    }

    public Chunk GetChunkAtPosition(Vector2Int chunkPosition)
    {
        if (!ChunkExistsAtPosition(chunkPosition))
            return null;

        return _chunkDictionary[chunkPosition];
    }

    public float[,] GetChunkNoiseMapArrayWithPosition(Vector2Int chunkPosition)
    {
        if (!ChunkExistsAtPosition(chunkPosition))
        {
            return null;
        }

        return _chunkDictionary[chunkPosition].ChunkNoiseMapArray;
    }

    public bool ChunkExistsAtPosition(Vector2Int chunkPosition) => _chunkDictionary.ContainsKey(chunkPosition);

    private void CreateInitialChunk()
    {
        GenerateChunk(new Vector2Int(0, 0));
    }

    private void Chunk_OnPlayerEnteredChunkRange(object sender, EventArgs e)
    {
        Chunk enteredChunk = sender as Chunk;

        if(!enteredChunk.IsFilled)
        {
            StartCoroutine(FillChunkCoroutine(enteredChunk));
        }

        LoadChunk(enteredChunk);

        List<Vector2Int> enteredChunkNeighbourPositionList = enteredChunk.NeighbourChunkList;

        foreach(Vector2Int chunkNeighbourPosition in enteredChunkNeighbourPositionList)
        {
            if(!_chunkDictionary.ContainsKey(chunkNeighbourPosition))
            {
                GenerateChunk(chunkNeighbourPosition);
            }
        }
    }

    private void Chunk_OnPlayerLeftChunkRange(object sender, EventArgs e)
    {
        Chunk exitChunk = sender as Chunk;


        if (!_loadingChunks.Contains(exitChunk) && exitChunk.IsLoadingTiles)
        {
            _loadingChunks.Add(exitChunk);
            exitChunk.OnFinishTileLoading += Chunk_OnFinishTileLoading;

            return;
        }

        UnloadChunk(exitChunk);
    }

    private void LoadChunk(Chunk chunk)
    {
        chunk.LoadChunk();
    }

    private void UnloadChunk(Chunk chunk)
    {
        chunk.UnloadChunk();
    }

    private void Chunk_OnFinishTileLoading(object sender, Chunk.OnFinishTileLoadingEventArgs e)
    {
        Chunk chunk = e.chunk;

        if (!chunk.IsPlayerInRange)
            UnloadChunk(chunk);

        chunk.OnFinishTileLoading -= Chunk_OnFinishTileLoading;
        _loadingChunks.Remove(chunk);
    }

    private Chunk GenerateChunk(Vector2Int chunkCenter, bool loadingExistingChunk = false)
    {
        Vector2 chunkPosition = new Vector2(chunkCenter.x, chunkCenter.y);
        Chunk chunk = Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);

        chunk.transform.gameObject.name = $"Chunk ({chunkCenter.x} | {chunkCenter.y})";
        chunk.transform.SetParent(_chunkParent);
        chunk.InitializeChunk(_chunkLayerCount);

        if (_chunkDictionary.ContainsKey(chunkCenter))
        {
            Debug.Log($"Chunk at {chunkCenter} is already in dictionary! Assuming we are loading chunk from save file...");
        }
        else
        {
            _chunkDictionary.Add(chunkCenter, chunk);
        }

        chunk.GenerateChunkNoiseMap(_seed, _noiseScale, _octaves, _persistance, _lacunarity, chunkCenter + _seamOffset, _normalizeMode);

        if(!loadingExistingChunk)
            GenerateNodePositionMap(chunk);

        return chunk;
    }

    private void GenerateNodePositionMap(Chunk parentChunk)
    {
        List<Vector2Int> nodeValidPositions = GetNodeValidPositions(parentChunk);
        Dictionary<ResourceNodeSpawnStruct, int>  nodesToSpawnDictionary = GetNodesToSpawn(_nodeResourceSpawnConfigSO, out int nodeAmount);

        if (nodeAmount > nodeValidPositions.Count)
        {
            Debug.LogError("Too much nodes for chunk!");
            return;
        }

        foreach(KeyValuePair<ResourceNodeSpawnStruct, int> keyValuePair in nodesToSpawnDictionary)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                int randomNodePosition = UnityEngine.Random.Range(0, nodeValidPositions.Count);

                parentChunk.AddNodeOnMap(nodeValidPositions[randomNodePosition], keyValuePair.Key.resourceNodePrefab);
                nodeValidPositions.RemoveAt(randomNodePosition);
            }
        }
    }

    private Dictionary<ResourceNodeSpawnStruct, int> GetNodesToSpawn(NodeResourceSpawnConfigSO nodeResourceSpawnConfigSO, out int nodeAmount)
    {
        Dictionary<ResourceNodeSpawnStruct, int> nodesToSpawnDictionary = new Dictionary<ResourceNodeSpawnStruct, int>();
        nodeAmount = 0;

        foreach (ResourceNodeSpawnStruct nodeSpawnStruct in nodeResourceSpawnConfigSO.NodeResourceSpawn)
        {
            if (UnityEngine.Random.Range(0f, 1f) > nodeSpawnStruct.spawnChance)
                continue;

            int nodeAmountForChunk = UnityEngine.Random.Range(nodeSpawnStruct.minPerChunk, nodeSpawnStruct.maxPerChunk + 1);

            nodesToSpawnDictionary.Add(nodeSpawnStruct, nodeAmountForChunk);
            nodeAmount += nodeAmountForChunk;
        }

        return nodesToSpawnDictionary;
    }

    private List<Vector2Int> GetNodeValidPositions(Chunk parentChunk)
    {
        if (!parentChunk.IsNoiseMapFilled)
        {
            Debug.LogError($"{parentChunk.name}'s noise map  wasn't filled!");
            return null;
        }

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

    private IEnumerator FillChunkCoroutine(Chunk parentChunk)
    {
        if (parentChunk.IsLoadingTiles)
            yield break;

        if (!parentChunk.IsNoiseMapFilled)
        {
            Debug.LogError($"{parentChunk.name}'s noise map  wasn't filled!");
            yield break;
        }

        parentChunk.StartLoadingTiles();

        if(!parentChunk.IsFilled)
        {
            Vector2Int chunkCenter = parentChunk.GetChunkPositionVector2Int();

            int loopIndexX = 0;
            int loopIndexY = 0;


            for (int x = chunkCenter.x - _chunkLayerCount; x <= chunkCenter.x + _chunkLayerCount; x++)
            {
                for (int y = chunkCenter.y - _chunkLayerCount; y <= chunkCenter.y + _chunkLayerCount; y++)
                {
                    float chunkNoiseMapValue = parentChunk.GetChunkNoiseMapValueWithXY(loopIndexX, loopIndexY);
                    Vector2Int tilPlacementPosition = new Vector2Int(x, y);

                    TileBase tile = GetRandomTile(chunkNoiseMapValue, out ETileType tileType);
                    TilePlacer.Instance.SetTileAtPosition(tile, tilPlacementPosition, tileType);

                    parentChunk.AddTileToChunk(tile, tilPlacementPosition, tileType);

                    loopIndexY++;
                }

                loopIndexX++;
                loopIndexY = 0;

                yield return new WaitForEndOfFrame();
            }

            parentChunk.FillChunk();
        }

        yield return PlaceResourceNodesCoroutine(parentChunk);

        parentChunk.FinishLoadingTiles();
    }

    private IEnumerator PlaceResourceNodesCoroutine(Chunk parentChunk)
    {
        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in parentChunk.NodePositionMapDictionary)
        {
            ResourceNode resourceNode = Instantiate(keyValuePair.Value, new Vector3(keyValuePair.Key.x, keyValuePair.Key.y, 0f), Quaternion.identity);
            parentChunk.AddNodeToChunk(keyValuePair.Key, resourceNode);
            resourceNode.transform.SetParent(parentChunk.transform);

            yield return new WaitForEndOfFrame();
        }
    }

    private ETileType GetTileTypeWithNoise(float tileNoise)
    {
        if (tileNoise <= _obstacleSpawnThreshold)
        {
            return ETileType.Obstacle;
        }
        else
        {
            return ETileType.Ground;
        }
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