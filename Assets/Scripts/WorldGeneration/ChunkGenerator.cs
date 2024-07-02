using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Tilemaps;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }

    private Dictionary<Vector2Int, Chunk> _chunkDictionary = new Dictionary<Vector2Int, Chunk>();

    [Header("Tile Setup")]
    [SerializeField] private TileConfigSO _tileConfigSO;

    [Header("Chunk Setup")]
    [SerializeField] private Transform _chunkParent;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private int _chunkLayerCount;

    [Header("Chunk Config")]
    [SerializeField] private NodeResourceSpawnConfigSO _nodeResourceSpawnConfigSO;

    [Header("Map Gentration Settings")]
    [SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves;

    [Range(0, 1)]
    [SerializeField] private float _persistance;

    [SerializeField] private float _lacunarity;
    [SerializeField] private float _obstacleChance;
    [SerializeField] private int _seed;
    [SerializeField] Vector2 seamOffset = Vector2.zero;
    [SerializeField] private Noise.NormalizeMode _normalizeMode;

    [Header("Debug Fields")]
    [SerializeField] private bool _showGizmos = false;
    [SerializeField] private List<Chunk> _loadingChunks = new List<Chunk>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Chunk.OnPlayerEnteredChunkRange += Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange += Chunk_OnPlayerLeftChunkRange;

        CreateInitialChunk();
    }

    private void OnDestroy()
    {
        Chunk.OnPlayerEnteredChunkRange -= Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange -= Chunk_OnPlayerLeftChunkRange;
    }

    public Chunk GetChunkAtPosition(Vector2Int chunkPosition)
    {
        bool chunkExists = ChunkExistsAtPosition(chunkPosition);

        if (!chunkExists)
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

        if(!enteredChunk.IsFilled())
        {
            StartCoroutine(FillChunkWithTilesCoroutine(enteredChunk));
        }

        LoadChunk(enteredChunk);

        List<Vector2Int> enteredChunkNeighbourPositionList = enteredChunk.NeighbourChunkList;

        foreach(Vector2Int chunkNeighbourPosition in enteredChunkNeighbourPositionList)
        {
            if(!_chunkDictionary.ContainsKey(chunkNeighbourPosition))
            {
                GenerateEmptyChunk(chunkNeighbourPosition);
            }
        }
    }

    private void Chunk_OnPlayerLeftChunkRange(object sender, EventArgs e)
    {
        Chunk exitChunk = sender as Chunk;


        if (!_loadingChunks.Contains(exitChunk) && exitChunk.IsLoadingTiles())
        {
            _loadingChunks.Add(exitChunk);
            exitChunk.OnFinishTileLoading += ExitChunk_OnFinishTileLoading;

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

    private void ExitChunk_OnFinishTileLoading(object sender, Chunk.OnFinishTileLoadingEventArgs e)
    {
        Chunk chunk = e.chunk;

        if (!chunk.IsPlayerInRange())
            UnloadChunk(chunk);

        chunk.OnFinishTileLoading -= ExitChunk_OnFinishTileLoading;
        _loadingChunks.Remove(chunk);
    }

    private Chunk GenerateChunk(Vector2Int chunkCenter)
    {
        Chunk chunk = GenerateEmptyChunk(chunkCenter);

        StartCoroutine(FillChunkWithTilesCoroutine(chunk));

        return chunk;
    }

    private Chunk GenerateEmptyChunk(Vector2Int chunkCenter)
    {
        Vector3 chunkPosition = new Vector3(chunkCenter.x, chunkCenter.y, 0);
        Transform chunkTransform = Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);

        chunkTransform.gameObject.name = $"Chunk  ({chunkCenter.x} | {chunkCenter.y})";

        chunkTransform.SetParent(_chunkParent);

        Chunk chunk = chunkTransform.GetComponent<Chunk>();
        chunk.InitializeChunkData(_chunkLayerCount);

        if(!_chunkDictionary.ContainsKey(chunkCenter))
        {
            _chunkDictionary.Add(chunkCenter, chunk);
        }

        chunk.GenerateChunkMap(_seed, _noiseScale, _octaves, _persistance, _lacunarity, chunkCenter + seamOffset, _normalizeMode);

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

        foreach (ResourceNodeSpawnStruct nodeSpawnStruct in nodeResourceSpawnConfigSO.nodeResourceSpawn)
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
        if (!parentChunk.ChunkMapFilled())
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
                float chunkNoiseMapValue = parentChunk.GetChunkMapValue(loopIndexX, loopIndexY);
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

    private IEnumerator FillChunkWithTilesCoroutine(Chunk parentChunk)
    {
        if (parentChunk.IsLoadingTiles())
            yield break;

        if (!parentChunk.ChunkMapFilled())
        {
            Debug.LogError($"{parentChunk.name}'s noise map  wasn't filled!");
            yield break;
        }

        parentChunk.StartLoadingTiles();

        Vector2Int chunkCenter = parentChunk.GetChunkPositionVector2Int();

        int loopIndexX = 0;
        int loopIndexY = 0;


        for (int x = chunkCenter.x - _chunkLayerCount; x <= chunkCenter.x + _chunkLayerCount; x++)
        {
            for (int y = chunkCenter.y - _chunkLayerCount; y <= chunkCenter.y + _chunkLayerCount; y++)
            {

                float chunkNoiseMapValue = parentChunk.GetChunkMapValue(loopIndexX, loopIndexY);

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

        yield return PlaceResourceNodes(parentChunk);

        parentChunk.FinishLoadingTiles();

    }

    private IEnumerator PlaceResourceNodes(Chunk parentChunk)
    {
        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in parentChunk.NodePositionMapDictionary)
        {
            ResourceNode resourceNode = Instantiate(keyValuePair.Value, new Vector3(keyValuePair.Key.x, keyValuePair.Key.y, 0f), Quaternion.identity);
            parentChunk.AddObjectToChunk(keyValuePair.Key, resourceNode.gameObject);
            resourceNode.transform.SetParent(parentChunk.transform);

            yield return new WaitForEndOfFrame();
        }
    }

    private ETileType GetTileTypeWithNoise(float tileNoise)
    {
        if (tileNoise <= _obstacleChance)
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
            return _tileConfigSO.obstacleTiles[0];
        }

        if(tileType == ETileType.Ground)
        {
            float placeDefaultTileChance = UnityEngine.Random.Range(0f, 1f);
            int randomTile;

            if (placeDefaultTileChance <= .5f)
                randomTile = 0;
            else
                randomTile = UnityEngine.Random.Range(1, _tileConfigSO.groundTiles.Count);

            return _tileConfigSO.groundTiles[randomTile];
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
