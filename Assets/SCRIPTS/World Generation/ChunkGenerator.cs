using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Tilemaps;
using Unity.Netcode;
using Youregone.Utilities;

public class ChunkGenerator : NetworkBehaviour
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
    //[SerializeField] private bool _useRandomSeed = true;

    [Header("Debug Fields")]
    [SerializeField] private bool _showGizmos = false;
    [SerializeField] private Dictionary<Vector2Int, Chunk> _chunkDictionary = new();

    private int _chunkSideLength => _chunkLayerCount * 2 + 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if(IsHost)
            CreateChunk(Vector2Int.zero);

        Debug.Log("Generator Spawned");
    }

    private void CreateChunk(Vector2Int position)
    {
        Chunk chunk = Instantiate(_chunkPrefab, new Vector2(position.x, position.y), Quaternion.identity);
        chunk.gameObject.name = $"Chunk {position.x}|{position.y}";

        chunk.GenerateNoiseMap(_chunkSideLength, _seed, _noiseScale, _octaves, _persistance, _lacunarity, position, _normalizeMode);
        _chunkDictionary.Add(position, chunk);

        GenerateNodePositionMap(chunk);

        chunk.NetworkObject.Spawn();
        chunk.transform.SetParent(_chunkParent);
    }

    public void PlayerEnteredChunkRange(Chunk chunk)
    {
        StartCoroutine(PlayerEnteredChunkRangeCoroutine(chunk));
    }

    public IEnumerator PlayerEnteredChunkRangeCoroutine(Chunk chunk)
    {
        if(!IsHost && !chunk.IsNoiseMapFilled)
        {
            chunk.GenerateNoiseMap(_chunkSideLength, _seed, _noiseScale, _octaves, _persistance, _lacunarity, chunk.Position, _normalizeMode);
        }

        if (!chunk.IsChunkDataGenerated)
        {
            yield return GenerateChunkTileDataCoroutine(chunk);

            SpawnNodesForChunkServerRpc(chunk.NetworkObject);
        }

        chunk.LoadChunk();

        GenerateChunkNeighboursServerRpc(chunk.NetworkObject);
    }

    [Rpc(SendTo.Server)]
    private void GenerateChunkNeighboursServerRpc(NetworkObjectReference chunkNetworkObjectReference)
    {
        if (!chunkNetworkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.LogError($"Couldn't unpack ChunkNetworkObjectReference");
            return;
        }

        Chunk chunk = networkObject.GetComponent<Chunk>();
        List<Vector2Int> chunkNeighbourPositions = GetChunkNeighbourPositions(chunk);

        foreach (Vector2Int position in chunkNeighbourPositions)
        {
            if (!_chunkDictionary.ContainsKey(position))
            {
                CreateChunk(position);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnNodesForChunkServerRpc(NetworkObjectReference chunkNetworkObjectReference)
    {
        if (!chunkNetworkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.LogError($"Couldn't unpack ChunkNetworkObjectReference");
            return;
        }

        Chunk chunk = networkObject.GetComponent<Chunk>();

        if (!chunk.AreNodesSpawned)
        {
            SpawnResourceNodes(chunk);
        }

        NetworkObjectReference[] nodeNetworkObjectReferenceArray = new NetworkObjectReference[chunk.SpawnedNodesDictionary.Count];

        int iterator = 0;
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in chunk.SpawnedNodesDictionary)
        {
            nodeNetworkObjectReferenceArray[iterator] = keyValuePair.Value.NetworkObject;
            iterator++;
        }

        UpdateSpawnedNodeDictionaryClientRpc(chunkNetworkObjectReference, nodeNetworkObjectReferenceArray);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateSpawnedNodeDictionaryClientRpc(NetworkObjectReference chunkNetworkObjectReference,
                                                      NetworkObjectReference[] nodeNetworkObjectReferenceArray)
    {
        if (!chunkNetworkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.LogError($"Couldn't unpack ChunkNetworkObjectReference");
            return;
        }

        Chunk chunk = networkObject.GetComponent<Chunk>();

        if (!IsHost)
        {
            Dictionary<Vector2Int, ResourceNode> nodeDictionary = new();

            for (int i = 0; i < nodeNetworkObjectReferenceArray.Length; i++)
            {
                if (nodeNetworkObjectReferenceArray[i].TryGet(out networkObject))
                {
                    ResourceNode node = networkObject.GetComponent<ResourceNode>();
                    nodeDictionary.Add(new Vector2Int((int)node.transform.position.x, (int)node.transform.position.y), node);
                }
                else
                {
                    Debug.LogError($"Couldn't unpack ChunkNetworkObjectReference");
                    return;
                }
            }

            chunk.FinishSpawningNodes(nodeDictionary);
        }

        if(!chunk.LocalPlayerInRange)
            chunk.UnloadNodes();
    }

    public void PlayerLeftChunkRange(Chunk chunk)
    {
        if (!chunk.IsLoadingTiles && !chunk.IsGeneratingTileData)
            chunk.UnloadChunk();
    }

    private List<Vector2Int> GetChunkNeighbourPositions(Chunk chunk)
    {
        List<Vector2Int> chunkNeighbourPositions = new();
        Vector2Int chunkCenter = chunk.Position;

        for (int x = chunkCenter.x - _chunkSideLength; x <= chunkCenter.x + _chunkSideLength; x += _chunkSideLength)
            for (int y = chunkCenter.y - _chunkSideLength; y <= chunkCenter.y + _chunkSideLength; y += _chunkSideLength)
            {
                if (_chunkDictionary.ContainsKey(new Vector2Int(x, y)))
                    continue;

                chunkNeighbourPositions.Add(new Vector2Int(x, y));
            }


        return chunkNeighbourPositions;
    }

    private void GenerateNodePositionMap(Chunk parentChunk)
    {
        List<Vector2Int> nodeValidPositions = GetNodeValidPositions(parentChunk);
        Dictionary<ResourceNode, int> nodesToSpawnDictionary = GetNodesToSpawn(_nodeResourceSpawnConfigSO, out int nodesAmount);

        if (nodesAmount > nodeValidPositions.Count)
        {
            UnityEngine.Debug.LogError("Too much nodes for chunk!");
            return;
        }

        Dictionary<Vector2Int, ResourceNode> resourceNodeMapDictionary = new();

        foreach (KeyValuePair<ResourceNode, int> keyValuePair in nodesToSpawnDictionary)
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
        Vector2Int chunkCenter = parentChunk.Position;
        List<Vector2Int> validNodePositionsList = new();

        int loopIndexX = 0;
        int loopIndexY = 0;

        for (int x = chunkCenter.x - _chunkLayerCount; x <= chunkCenter.x + _chunkLayerCount; x++)
        {
            for (int y = chunkCenter.y - _chunkLayerCount; y <= chunkCenter.y + _chunkLayerCount; y++)
            {
                float chunkNoiseMapValue = parentChunk.NoiseMapArray[loopIndexX, loopIndexY];
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

    private IEnumerator GenerateChunkTileDataCoroutine(Chunk parentChunk)
    {
        if (parentChunk.IsGeneratingTileData)
            yield break;

        parentChunk.StartGeneratingTileData();

        List<TileData> tileDataMapList = new();
        Vector2Int chunkCenter = parentChunk.Position;

        int loopIndexX = 0;
        int loopIndexY = 0;

        for (int x = chunkCenter.x - _chunkLayerCount; x <= chunkCenter.x + _chunkLayerCount; x++)
        {
            for (int y = chunkCenter.y - _chunkLayerCount; y <= chunkCenter.y + _chunkLayerCount; y++)
            {
                float chunkNoiseMapValue = parentChunk.NoiseMapArray[loopIndexX, loopIndexY];

                Vector2Int tilPlacementPosition = new(x, y);
                TileBase tile = GetRandomTile(chunkNoiseMapValue, out ETileType tileType);
                TileData tileData = new(tile, tilPlacementPosition, tileType);

                tileDataMapList.Add(tileData);
                loopIndexY++;
            }

            loopIndexX++;
            loopIndexY = 0;

            yield return new WaitForEndOfFrame();
        }

        parentChunk.FinishGeneratingTileData(tileDataMapList);
    }

    private void SpawnResourceNodes(Chunk parentChunk)
    {
        if (parentChunk.IsSpawningNodes)
            return;

        parentChunk.StartSpawningNodes();

        Dictionary<Vector2Int, ResourceNode> nodeDictionary = new();

        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in parentChunk.NodePrefabMapDictionary)
        {
            ResourceNode resourceNode = Instantiate(keyValuePair.Value, new Vector2(keyValuePair.Key.x, keyValuePair.Key.y), Quaternion.identity);
            resourceNode.NetworkObject.Spawn();
            nodeDictionary.Add(keyValuePair.Key, resourceNode);
            resourceNode.transform.SetParent(parentChunk.transform);
        }

        parentChunk.FinishSpawningNodes(nodeDictionary);
    }

    private ETileType GetTileTypeWithNoise(float tileNoise)
    {
        return tileNoise <= _obstacleSpawnThreshold ? ETileType.Obstacle : ETileType.Ground;
    }

    private TileBase GetRandomTile(float tileNoise, out ETileType tileType)
    {
        tileType = GetTileTypeWithNoise(tileNoise);

        if (tileType == ETileType.Obstacle)
        {
            return _tileConfigSO.ObstacleTiles[0];
        }

        if (tileType == ETileType.Ground)
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

        foreach (KeyValuePair<Vector2Int, Chunk> keyValuePair in _chunkDictionary)
        {
            Gizmos.DrawSphere((Vector2)keyValuePair.Key, .5f);
        }
    }
}