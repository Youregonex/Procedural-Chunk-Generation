using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;

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
    [SerializeField] private int _chunkLayerCount;
    [SerializeField] private bool _isLoaded;
    [SerializeField] private bool _isPlayerInRange;
    [SerializeField] private bool _isFilled = false;
    [SerializeField] private bool _isLoadingTiles = false;
    [SerializeField] private bool _chunkMapFilled;
    [SerializeField] private float[,] _chunkNoiseMapArray;

    [SerializeField] private Dictionary<Vector2Int, ResourceNode> _nodePositionMapDictionary = new Dictionary<Vector2Int, ResourceNode>();
    [SerializeField] private Dictionary<Vector2Int, GameObject> _chunkObjectDictionary = new Dictionary<Vector2Int, GameObject>();
    [SerializeField] private List<Vector2Int> _neighbourChunkList = new List<Vector2Int>();
    [SerializeField] private List<TileData> _chunkTilesList = new List<TileData>();

    public Dictionary<Vector2Int, ResourceNode> NodePositionMapDictionary => _nodePositionMapDictionary;
    public List<Vector2Int> NeighbourChunkList => _neighbourChunkList;
    public Dictionary<Vector2Int, GameObject> ChunkObjectDictionary => _chunkObjectDictionary;
    public List<TileData> ChunkTilesList => _chunkTilesList;
    public float[,] ChunkNoiseMapArray => _chunkNoiseMapArray;

    private int _sideLength;


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

    public void InitializeChunkData(int chunkLayerCount)
    {
        _chunkLayerCount = chunkLayerCount;

        _sideLength = (_chunkLayerCount * 2) + 1;

        for (int x = (int)transform.position.x + -_sideLength; x <= (int)transform.position.x + _sideLength; x += _sideLength)
        {
            for (int y = (int)transform.position.y + -_sideLength; y <= (int)transform.position.y + _sideLength; y += _sideLength)
            {
                if (new Vector3(x, y) == transform.position)
                    continue;

                Vector2Int neighbourChunkPosition = new Vector2Int(x, y);

                _neighbourChunkList.Add(neighbourChunkPosition);
            }
        }
    }

    public void GenerateChunkMap(int seed, float noiseScale, int octaves, float persistance, float lacunarity, Vector2 offset, Noise.NormalizeMode normalizeMode)
    {
        if (_chunkMapFilled)
            return;

        _chunkNoiseMapArray = new float[_sideLength, _sideLength];

        _chunkNoiseMapArray = Noise.GenerateNoiseMap(_sideLength, _sideLength, seed, noiseScale, octaves, persistance, lacunarity, offset, normalizeMode);

        _chunkMapFilled = true;
    }

    public void UnloadChunk()
    {
        _isLoaded = false;

        foreach(TileData tileData in _chunkTilesList)
        {
            TilePlacer.Instance.ClearTileAtPosition(tileData.TilePosition, tileData.TileType);
        }

        foreach(KeyValuePair<Vector2Int, GameObject> keyValuePair in _chunkObjectDictionary)
        {
            keyValuePair.Value.SetActive(false);
        }
    }

    public void LoadChunk()
    {
        _isLoaded = true;

        foreach (TileData tileData in _chunkTilesList)
        {
            TilePlacer.Instance.SetTileAtPosition(tileData.Tile, tileData.TilePosition, tileData.TileType);
        }

        foreach (KeyValuePair<Vector2Int, GameObject> keyValuePair in _chunkObjectDictionary)
        {
            keyValuePair.Value.SetActive(true);
        }
    }

    public void AddObjectToChunk(Vector2Int objectPosition, GameObject newObject)
    {
        _chunkObjectDictionary.Add(objectPosition, newObject);
    }

    public void AddNodeOnMap(Vector2Int nodePosition, ResourceNode resourceNodePrefab)
    {
        _nodePositionMapDictionary.Add(nodePosition, resourceNodePrefab);
    }

    public void AddTileToChunk(TileBase tile, Vector2Int tilePosition, ETileType tileType)
    {
        _chunkTilesList.Add(new TileData(tile, tilePosition, tileType));
    }

    public bool IsFilled() => _isFilled;
    public bool IsLoaded() => _isLoaded;
    public void FillChunk() => _isFilled = true;
    public bool IsLoadingTiles() => _isLoadingTiles;
    public bool ChunkMapFilled() => _chunkMapFilled;
    public bool IsPlayerInRange() => _isPlayerInRange;
    public float GetChunkMapValue(int x, int y) => _chunkNoiseMapArray[x, y];
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
    }
}
