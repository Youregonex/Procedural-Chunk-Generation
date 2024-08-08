using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Chunk : MonoBehaviour
{
    public static event EventHandler OnPlayerEnteredChunkRange;
    public static event EventHandler OnPlayerLeftChunkRange;

    public event EventHandler<OnFinishTileLoadingEventArgs> OnFinishTileLoading;
    public class OnFinishTileLoadingEventArgs : EventArgs
    {
        public Chunk chunk;
    }

    [Header("Debug Fields")]
    // Chunk data
    [SerializeField] private bool _isLoaded = false;
    [SerializeField] private bool _isPlayerInRange = false;
    [SerializeField] private bool _nodesSpawned = false;
    [SerializeField] private bool _isLoadingTiles = false;
    [SerializeField] private bool _noiseMapFilled = false;
    [SerializeField] private float[,] _noiseMapArray;

    // Collections
    [SerializeField] private List<Vector2Int> _neighbourChunkList = new List<Vector2Int>();
    [SerializeField] private SerializableDictionary<Vector2Int, ResourceNode> _nodePositionMapDictionary = new SerializableDictionary<Vector2Int, ResourceNode>();
    [SerializeField] private SerializableDictionary<Vector2Int, ResourceNode> _chunkNodeDictionary = new SerializableDictionary<Vector2Int, ResourceNode>();

    // Properties
    public Dictionary<Vector2Int, ResourceNode> NodePositionMapDictionary => _nodePositionMapDictionary;
    public Dictionary<Vector2Int, ResourceNode> ChunkObjectDictionary => _chunkNodeDictionary;
    public List<Vector2Int> NeighbourChunkList => _neighbourChunkList;
    public float[,] ChunkNoiseMapArray => _noiseMapArray;

    public bool IsLoaded => _isLoaded;
    public bool NodesSpawned => _nodesSpawned;
    public bool IsLoadingTiles => _isLoadingTiles;
    public bool IsNoiseMapFilled => _noiseMapFilled;
    public bool IsPlayerInRange => _isPlayerInRange;

    private int _sideLength;

    private void OnDestroy()
    {
        foreach(KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _chunkNodeDictionary)
        {
            keyValuePair.Value.OnDepletion -= Node_OnDepletion;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerChunkInteraction>() != null)
        {
            OnPlayerEnteredChunkRange?.Invoke(this, EventArgs.Empty);
            _isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerChunkInteraction>() != null)
        {
            OnPlayerLeftChunkRange?.Invoke(this, EventArgs.Empty);
            _isPlayerInRange = false;
        }
    }

    public ChunkSaveData GenerateSaveData()
    {
        Vector2Int chunkPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        ChunkSaveData chunkSaveData = new ChunkSaveData(chunkPosition,
                                                        _nodePositionMapDictionary);
        return chunkSaveData;
    }

    public Chunk LoadChunkFromSaveData(ChunkSaveData chunkSaveData)
    {
        _nodesSpawned = false;
        _nodePositionMapDictionary = chunkSaveData.nodePositionMapDictionary;

        return this;
    }

    public void InitializeChunk(int chunkLayerCount)
    {
        _sideLength = (chunkLayerCount * 2) + 1;

        UpdateChunkNeighbourPositions();
    }

    public void GenerateChunkNoiseMap(int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset, Noise.NormalizeMode normalizeMode)
    {
        if (_noiseMapFilled)
        {
            Debug.LogError($"{gameObject.name} noise map is already filled!");
            return;
        }

        _noiseMapArray = new float[_sideLength, _sideLength];

        _noiseMapArray = Noise.GenerateNoiseMap(_sideLength,
                                                _sideLength,
                                                seed,
                                                noiseScale,
                                                octaves,
                                                persistance,
                                                lacunarity,
                                                offset,
                                                normalizeMode);

        _noiseMapFilled = true;
    }

    public void UnloadChunk()
    {
        _isLoaded = false;

        UnloadTiles();
        UnloadNodes();
    }

    public void LoadChunk()
    {
        _isLoaded = true;

        LoadTiles();
        LoadNodes();
    }

    public void AddNodeToChunk(Vector2Int objectPosition, ResourceNode node)
    {
        _chunkNodeDictionary.Add(objectPosition, node);
        node.OnDepletion += Node_OnDepletion;
    }

    public void AddNodeOnMap(Vector2Int nodePosition, ResourceNode resourceNodePrefab)
    {
        _nodePositionMapDictionary.Add(nodePosition, resourceNodePrefab);
    }

    public void SpawnNodes() => _nodesSpawned = true;
    public float GetChunkNoiseMapValueWithXY(int x, int y) => _noiseMapArray[x, y];
    public Vector2Int GetChunkPositionVector2Int() => new Vector2Int((int)transform.position.x, (int)transform.position.y);

    public void StartLoadingTiles()
    {
        _isLoadingTiles = true;
    }

    public void FinishLoadingTiles()
    {
        _isLoadingTiles = false;

        OnFinishTileLoading?.Invoke(this, new OnFinishTileLoadingEventArgs
        {
            chunk = this
        });

        if (!_isPlayerInRange)
            UnloadChunk();
    }

    private void UpdateChunkNeighbourPositions()
    {
        for (int x = (int)transform.position.x + -_sideLength; x <= (int)transform.position.x + _sideLength; x += _sideLength)
            for (int y = (int)transform.position.y + -_sideLength; y <= (int)transform.position.y + _sideLength; y += _sideLength)
            {
                if (new Vector2(x, y) == (Vector2)transform.position)
                    continue;

                Vector2Int neighbourChunkPosition = new Vector2Int(x, y);
                _neighbourChunkList.Add(neighbourChunkPosition);
            }
    }

    private void Node_OnDepletion(Vector2Int nodePosition)
    {
        _chunkNodeDictionary[nodePosition].OnDepletion -= Node_OnDepletion;

        _nodePositionMapDictionary.Remove(nodePosition);
        _chunkNodeDictionary.Remove(nodePosition);
    }

    private void UnloadTiles()
    {
        TilePlacer.Instance.UnloadChunkTiles(this);
    }

    private void UnloadNodes()
    {
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _chunkNodeDictionary)
        {
            keyValuePair.Value.gameObject.SetActive(false);
        }
    }

    private void LoadTiles()
    {
        TilePlacer.Instance.LoadChunkTiles(this);
    }

    private void LoadNodes()
    {
        foreach (KeyValuePair<Vector2Int, ResourceNode> keyValuePair in _chunkNodeDictionary)
        {
            keyValuePair.Value.gameObject.SetActive(true);
        }
    }
}

[Serializable]
public struct ChunkSaveData
{
    public Vector2Int position;

    public SerializableDictionary<Vector2Int, ResourceNode> nodePositionMapDictionary;

    public ChunkSaveData(Vector2Int position,
                         SerializableDictionary<Vector2Int, ResourceNode> nodePositionMapDictionary)
    {
        this.position = position;
        this.nodePositionMapDictionary = nodePositionMapDictionary;
    }
}