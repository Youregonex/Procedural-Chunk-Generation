using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.Netcode;
using Youregone.Utilities;

[Serializable]
public class Chunk : NetworkBehaviour, IGenerateSaveData
{
    public event Action<Chunk> OnFinishTileLoading;

    [Header("Debug Fields")]
    // Chunk data
    [SerializeField] private NetworkVariable<bool> _tileDataMapFilled = new(false);
    [SerializeField] private NetworkVariable<bool> _nodesSpawned = new(false);

    // Collections
    [SerializeField] private List<Vector2Int> _neighbourChunkList = new();
    [SerializeField] private List<PlayerCore> _playersInRangeList = new();
    [SerializeField] private Dictionary<Vector2Int, ResourceNode> _nodePositionMapDictionary = new();
    [SerializeField] private Dictionary<Vector2Int, ResourceNode> _chunkNodeDictionary = new();
    [SerializeField] private List<TileData> _tileDataMapList = new();

    // Properties
    [field: SerializeField] public NetworkVariable<float[]> NoiseMapArray { get; private set; } = new();
    public Dictionary<Vector2Int, ResourceNode> NodePositionMapDictionary => _nodePositionMapDictionary;
    public Dictionary<Vector2Int, ResourceNode> ChunkObjectDictionary => _chunkNodeDictionary;
    public List<Vector2Int> NeighbourChunkList => _neighbourChunkList;

    public NetworkVariable<bool> GeneratingTileData { get; private set; } = new(false);
    public NetworkVariable<bool> SpawningNodes { get; private set; } = new(false);
    public NetworkVariable<bool> IsLoadingTiles { get; private set; } = new(false);

    public bool ChunkDataFilled => _tileDataMapFilled.Value && _nodesSpawned.Value;
    public bool IsAnyPlayerInRange => _playersInRangeList.Count > 0;

    private int _sideLength;

    public override void OnDestroy()
    {
        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _chunkNodeDictionary)
        {
            keyValuePair.Value.OnDepletion -= ResourceNode_OnDepletion;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerChunkInteraction playerChunkInteraction))
        {
            _playersInRangeList.Add(playerChunkInteraction.transform.root.GetComponent<PlayerCore>());
            ChunkGenerator.Instance.PlayerEnteredChunk(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerChunkInteraction playerChunkInteraction))
        {
            _playersInRangeList.Remove(playerChunkInteraction.transform.root.GetComponent<PlayerCore>());
            ChunkGenerator.Instance.PlayerLeftChunk(this);
        }
    }

    public SaveData GenerateSaveData()
    {
        Vector2Int chunkPosition = new((int)transform.position.x, (int)transform.position.y);
        ChunkSaveData chunkSaveData = new(chunkPosition,
                                         (SerializableDictionary<Vector2Int, ResourceNode>)_nodePositionMapDictionary);
        return chunkSaveData;
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        ChunkSaveData chunkSaveData = saveData as ChunkSaveData;

        _nodesSpawned.Value = false;
        _nodePositionMapDictionary = chunkSaveData.nodePositionMapDictionary;
    }

    public void InitializeChunk(int chunkLayerCount)
    {
        _sideLength = (chunkLayerCount * 2) + 1;

        UpdateChunkNeighbourPositions();
    }

    public void GenerateChunkNoiseMap(int seed,
                                      float noiseScale,
                                      int octaves,
                                      float persistance,
                                      float lacunarity,
                                      Vector2 offset,
                                      Noise.NormalizeMode normalizeMode)
    {
        float[,] md_noiseArray = new float[_sideLength, _sideLength];

        md_noiseArray = Noise.GenerateNoiseMap(_sideLength,
                                               _sideLength,
                                               seed,
                                               noiseScale,
                                               octaves,
                                               persistance,
                                               lacunarity,
                                               offset,
                                               normalizeMode);

        NoiseMapArray.Value = Utility.FlattenArray(md_noiseArray);
    }

    public void UnloadChunk()
    {
        if (IsAnyPlayerInRange)
            return;

        UnloadTiles();
        UnloadNodes();
    }

    public void LoadChunk()
    {
        StartCoroutine(LoadTiles());
        LoadNodes();

        FinishLoadingTiles();
    }

    public void FillNodeDictionary(Dictionary<Vector2Int, ResourceNode> nodeDictionary)
    {
        _chunkNodeDictionary = new(nodeDictionary);

        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in nodeDictionary)
        {
            keyValuePair.Value.OnDepletion += ResourceNode_OnDepletion;
        }

        _nodesSpawned.Value = true;
    }

    public void FillResourceNodeMap(Dictionary<Vector2Int, ResourceNode> resourceNodeMap)
    {
        _nodePositionMapDictionary = new(resourceNodeMap);
    }

    public void FillChunkTileDataMap(List<TileData> tileDataMap)
    {
        _tileDataMapList = new(tileDataMap);
        _tileDataMapFilled.Value = true;
    }

    public float GetChunkNoiseMapValueWithXY(int x, int y) => Utility.UnflattenArray(NoiseMapArray.Value, _sideLength, _sideLength)[x, y];
    public Vector2Int GetChunkPositionVector2Int() => new((int)transform.position.x, (int)transform.position.y);

    public void StartGeneratingTileData() => GeneratingTileData.Value = true;
    public void FinishGeneratingTileData() => GeneratingTileData.Value = false;
    public void StartSpawningNodes() => SpawningNodes.Value = true;
    public void FinishSpawningNodes() => SpawningNodes.Value = false;

    public void StartLoadingTiles()
    {
        IsLoadingTiles.Value = true;
    }

    public void FinishLoadingTiles()
    {
        IsLoadingTiles.Value = false;
        OnFinishTileLoading?.Invoke(this);

        UnloadChunk();
    }

    private void UpdateChunkNeighbourPositions()
    {
        for (int x = (int)transform.position.x + -_sideLength; x <= (int)transform.position.x + _sideLength; x += _sideLength)
            for (int y = (int)transform.position.y + -_sideLength; y <= (int)transform.position.y + _sideLength; y += _sideLength)
            {
                if (new Vector2(x, y) == (Vector2)transform.position)
                    continue;

                Vector2Int neighbourChunkPosition = new(x, y);
                _neighbourChunkList.Add(neighbourChunkPosition);
            }
    }

    private void ResourceNode_OnDepletion(Vector2Int nodePosition)
    {
        _chunkNodeDictionary[nodePosition].OnDepletion -= ResourceNode_OnDepletion;

        _nodePositionMapDictionary.Remove(nodePosition);
        _chunkNodeDictionary.Remove(nodePosition);
    }

    private void UnloadTiles()
    {
        TilePlacer.Instance.UnloadChunkTiles(_tileDataMapList);
    }

    private void UnloadNodes()
    {
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _chunkNodeDictionary)
        {
            keyValuePair.Value.gameObject.SetActive(false);
        }
    }

    private IEnumerator LoadTiles()
    {
        if (IsLoadingTiles.Value)
            yield break;

        StartLoadingTiles();

        int iterator = 0;

        for (int i = 0; i < _tileDataMapList.Count; i++)
        {
            TilePlacer.Instance.SetTile(_tileDataMapList[i]);

            iterator++;
            if(iterator == _sideLength)
            {
                iterator = 0;
                yield return new WaitForEndOfFrame();
            }
        }

        FinishLoadingTiles();
    }

    private void LoadNodes()
    {
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _chunkNodeDictionary)
        {
            keyValuePair.Value.gameObject.SetActive(true);
        }
    }
}