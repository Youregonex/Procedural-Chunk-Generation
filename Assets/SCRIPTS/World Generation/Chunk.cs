using System;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Youregone.Utilities;
using System.Collections;

[Serializable]
public class Chunk : NetworkBehaviour
{
    public event Action<Chunk> OnFinishGeneratingTileData;

    [field: Header("Debug Fields")]
    [field: SerializeField] public bool IsGeneratingTileData { get; private set; } = false;
    [field: SerializeField] public bool IsTileDataGenerated { get; private set; } = false;
    [field: SerializeField] public bool IsNoiseMapFilled { get; private set; } = false;
    [field: SerializeField] public bool IsLoadingTiles { get; private set; } = false;
    [field: SerializeField] public bool IsNodeMapFilled { get; private set; } = false;
    [field: SerializeField] public bool AreNodesSpawned { get; private set; } = false;
    [field: SerializeField] public bool IsSpawningNodes { get; private set; } = false;
    [SerializeField] private List<ResourceNode> _NODETEST;
    [SerializeField] private List<TileData> _tileDataList = new();
    [SerializeField] private Dictionary<Vector2Int, ResourceNode> _nodePrefabMapDictionary = new();
    [SerializeField] private Dictionary<Vector2Int, ResourceNode> _spawnedNodesDictionary = new();
    [SerializeField] private List<PlayerCore> _playersInChunkRange = new();

    private float[,] _noiseMapArray;
    private int _sideLength;

    public bool IsChunkDataGenerated => IsTileDataGenerated && AreNodesSpawned;
    public bool LocalPlayerInRange => _playersInChunkRange.Count > 0;
    public float[,] NoiseMapArray => _noiseMapArray;
    public Dictionary<Vector2Int, ResourceNode> SpawnedNodesDictionary => _spawnedNodesDictionary;
    public Dictionary<Vector2Int, ResourceNode> NodePrefabMapDictionary => _nodePrefabMapDictionary;
    public Vector2Int Position => new((int)transform.position.x, (int)transform.position.y);




    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerChunkInteraction playerChunkInteraction) && playerChunkInteraction.IsOwner)
        {
            _playersInChunkRange.Add(playerChunkInteraction.transform.root.GetComponent<PlayerCore>());
            ChunkGenerator.Instance.PlayerEnteredChunkRange(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerChunkInteraction playerChunkInteraction) && playerChunkInteraction.IsOwner)
        {
            _playersInChunkRange.Remove(playerChunkInteraction.transform.root.GetComponent<PlayerCore>());
            ChunkGenerator.Instance.PlayerLeftChunkRange(this);
        }
    }

    public void StartSpawningNodes()
    {
        IsSpawningNodes = true;
    }

    public void FinishSpawningNodes(Dictionary<Vector2Int, ResourceNode> nodeDictionary)
    {
        _spawnedNodesDictionary = nodeDictionary;
        AreNodesSpawned = true;
        IsSpawningNodes = false;

        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _spawnedNodesDictionary)
        {
            if (!_NODETEST.Contains(keyValuePair.Value))
                _NODETEST.Add(keyValuePair.Value);
        }

        if (!IsHost)
            return;

        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _spawnedNodesDictionary)
        {
            keyValuePair.Value.OnDepletion += ResourceNode_OnDepletion;
        }
    }

    private void ResourceNode_OnDepletion(Vector2Int resourceNodePosition)
    {
        _spawnedNodesDictionary[resourceNodePosition].OnDepletion -= ResourceNode_OnDepletion;

        RemoveNodeFromChunkClientRpc(resourceNodePosition);

        _NODETEST.Remove(_spawnedNodesDictionary[resourceNodePosition]);
        _spawnedNodesDictionary.Remove(resourceNodePosition);
        _nodePrefabMapDictionary.Remove(resourceNodePosition);
    }

    [Rpc(SendTo.NotServer)]
    private void RemoveNodeFromChunkClientRpc(Vector2Int position)
    {
        _NODETEST.Remove(_spawnedNodesDictionary[position]);
        _spawnedNodesDictionary.Remove(position);
    }

    public void LoadChunk()
    {
        if (IsLoadingTiles)
            return;

        LoadNodes();
        StartCoroutine(LoadTiles());
    }

    public void UnloadChunk()
    {
        UnloadNodes();
        UnloadTiles();
    }

    public IEnumerator LoadTiles()
    {
        IsLoadingTiles = true;

        int iterator = 0;

        for (int i = 0; i < _tileDataList.Count; i++)
        {
            TilePlacer.Instance.SetTile(_tileDataList[i]);
            iterator++;

            if(iterator == _sideLength)
            {
                yield return new WaitForEndOfFrame();
                iterator = 0;
            }
        }

        IsLoadingTiles = false;

        if (!LocalPlayerInRange)
            UnloadChunk();
    }

    public void UnloadTiles()
    {
        for (int i = 0; i < _tileDataList.Count; i++)
        {
            TilePlacer.Instance.ClearTile(_tileDataList[i]);
        }
    }

    public void LoadNodes()
    {
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _spawnedNodesDictionary)
        {
            keyValuePair.Value.gameObject.SetActive(true);
        }
    }

    public void UnloadNodes()
    {
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _spawnedNodesDictionary)
        {
            if(keyValuePair.Value != null)
                keyValuePair.Value.gameObject.SetActive(false);
        }
    }

    public void StartGeneratingTileData()
    {
        IsGeneratingTileData = true;
    }

    public void FinishGeneratingTileData(List<TileData> tileData)
    {
        _tileDataList = tileData;
        IsTileDataGenerated = true;
        IsGeneratingTileData = false;

        OnFinishGeneratingTileData?.Invoke(this);
    }

    public void FillResourceNodeMap(Dictionary<Vector2Int, ResourceNode> resourceNodeMapDictionary)
    {
        _nodePrefabMapDictionary = resourceNodeMapDictionary;

        IsNodeMapFilled = true;
    }

    public void GenerateNoiseMap(
        int chunkSideLength,
        int seed,
        float noiseScale,
        int octaves,
        float persistance,
        float lacunarity,
        Vector2 offset,
        Noise.NormalizeMode normalizeMode)
    {
        _sideLength = chunkSideLength;

        _noiseMapArray = Noise.GenerateNoiseMap(
            chunkSideLength,
            chunkSideLength,
            seed,
            noiseScale,
            octaves,
            persistance,
            lacunarity,
            offset,
            normalizeMode);

        IsNoiseMapFilled = true;
    }
}