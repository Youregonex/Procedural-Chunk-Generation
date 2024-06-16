using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Chunk : MonoBehaviour
{
    private List<Tile> _chunkTiles;

    public static event EventHandler OnPlayerEnteredChunkRange;
    public static event EventHandler OnPlayerLeftChunkRange;

    public event EventHandler<OnFinishTileLoadingEventArgs> OnFinishTileLoading;
    public class OnFinishTileLoadingEventArgs : EventArgs
    {
        public Chunk chunk;
    }

    private int _sideLength;
    [SerializeField] private int _tilesCount = 0;

    [Header("Debug Fields")]
    [SerializeField] private List<Vector2Int> _neighbourChunkList;
    [SerializeField] private int _chunkLayerCount;
    [SerializeField] private Transform _markerPrefab;
    [SerializeField] private bool _isLoaded;
    [SerializeField] private bool _isPlayerInRange;
    [SerializeField] private bool _isFilled = false;
    [SerializeField] private bool _isLoadingTiles = false;
    [SerializeField] private bool _chunkMapFilled;
    [SerializeField] private int[,] _chunkMap;

    private void Awake()
    {
        _chunkTiles = new List<Tile>();
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

                // For debug purposes
                Transform marker = Instantiate(_markerPrefab, new Vector3(x, y), Quaternion.identity);
                marker.SetParent(gameObject.transform);
            }
        }
    }

    public void GenerateChunkMap(int wallPercent, int smootheGenerations, int amountOfWallsToChangeTile)
    {
        if (_chunkMapFilled)
            return;

        _chunkMap = new int[_sideLength, _sideLength];

        for (int x = 0; x < _sideLength; x++)
        {
            for (int y = 0; y < _sideLength; y++)
            {
                _chunkMap[x, y] = UnityEngine.Random.Range(0, 101) > wallPercent ? 0 : 1;
            }
        }

        SmootheChunkMap(smootheGenerations, amountOfWallsToChangeTile);

        _chunkMapFilled = true;
    }

    private void SmootheChunkMap(int smootheGenerations, int amountOfWallsToChangeTile)
    {
        for (int i = 0; i < smootheGenerations; i++)
        {
            for (int x = 0; x < _sideLength; x++)
            {
                for (int y = 0; y < _sideLength; y++)
                {
                    int neighbourWallTilesAmount = GetSurroundingWallsCount(x, y);

                    if (neighbourWallTilesAmount > amountOfWallsToChangeTile)
                    {
                        _chunkMap[x, y] = 1;
                    }
                    else if (neighbourWallTilesAmount < amountOfWallsToChangeTile)
                    {
                        _chunkMap[x, y] = 0;
                    }
                }
            }
        }
    }

    private int GetSurroundingWallsCount(int x, int y)
    {
        int wallCount = 0;

        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                //if(neighbourX > _sideLength)
                //{

                //}

                if (neighbourX >= 0 && neighbourX < _sideLength && neighbourY >= 0 && neighbourY < _sideLength)
                {
                    if (neighbourX != x || neighbourY != y)
                    {
                        if (_chunkMap[neighbourX, neighbourY] == 1)
                            wallCount += _chunkMap[neighbourX, neighbourY];
                    }
                }
            }
        }

        return wallCount;
    }

    public void UnloadChunk()
    {
        _isLoaded = false;

        foreach(Tile tile in _chunkTiles)
        {
            tile.gameObject.SetActive(false);
        }
    }

    public void LoadChunk()
    {
        _isLoaded = true;

        foreach (Tile tile in _chunkTiles)
        {
            tile.gameObject.SetActive(true);
        }
    }

    public void AddTileToChunk(Tile tile)
    {
        _chunkTiles.Add(tile);
        _tilesCount++;
    }

    public Vector2Int GetChunkPositionVector2Int() => new Vector2Int((int)transform.position.x, (int)transform.position.y);
    public bool IsFilled() => _isFilled;
    public bool IsLoaded() => _isLoaded;
    public void FillChunk() => _isFilled = true;
    public List<Vector2Int> GetNeighbourChunkList() => _neighbourChunkList;
    public bool IsLoadingTiles() => _isLoadingTiles;
    public bool IsPlayerInRange() => _isPlayerInRange;
    public bool ChunkMapFilled() => _chunkMapFilled;
    public int GetChunkMapValue(int x, int y) => _chunkMap[x, y];

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
