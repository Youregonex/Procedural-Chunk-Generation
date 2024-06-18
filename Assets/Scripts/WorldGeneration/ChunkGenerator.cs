using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ChunkGenerator : MonoBehaviour
{
    public static ChunkGenerator Instance { get; private set; }

    private Dictionary<Vector2Int, Chunk> _chunkDictionary;
    [SerializeField] private List<Chunk> _loadingChunks;

    [Header("Tile Setup")]
    [SerializeField] private TileConfigSO _tileConfigSO;
    [SerializeField] private Transform _tilePrefab;

    [Header("Chunk Setup")]
    [SerializeField] private Transform _chunkParent;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private int _chunkLayerCount;

    [Header("Map Gentration Settings")]
    [SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves;

    [Range(0, 1)]
    [SerializeField] private float _persistance;

    [SerializeField] private float _lacunarity;
    [SerializeField] private float _obstacleChance;
    [SerializeField] private int _seed;
    [SerializeField] Vector2 seamOffset;
    [SerializeField] private Noise.NormalizeMode _normalizeMode;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;

        DontDestroyOnLoad(gameObject);

        _chunkDictionary = new Dictionary<Vector2Int, Chunk>();
        _loadingChunks = new List<Chunk>();
    }

    private void Start()
    {
        Chunk.OnPlayerEnteredChunkRange += Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange += Chunk_OnPlayerLeftChunkRange;

        CreateInitialChunk();
    }

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

        List<Vector2Int> enteredChunkNeighbourPositionList = enteredChunk.GetNeighbourChunkList();

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

        if(exitChunk.IsLoadingTiles())
        {
            if(!_loadingChunks.Contains(exitChunk))
            {
                _loadingChunks.Add(exitChunk);
                exitChunk.OnFinishTileLoading += ExitChunk_OnFinishTileLoading;
            }
        }
        else
        {
            UnLoadChunk(exitChunk);
        }
    }

    private void LoadChunk(Chunk chunk)
    {
        chunk.LoadChunk();
    }

    private void UnLoadChunk(Chunk chunk)
    {
        chunk.UnloadChunk();
    }

    private void ExitChunk_OnFinishTileLoading(object sender, Chunk.OnFinishTileLoadingEventArgs e)
    {
        Chunk chunk = e.chunk;

        if (!chunk.IsPlayerInRange())
            UnLoadChunk(chunk);

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

        return chunk;
    }

    private IEnumerator FillChunkWithTilesCoroutine(Chunk parentChunk)
    {
        if (parentChunk.IsLoadingTiles())
            yield break;

        parentChunk.StartLoadingTiles();

        Vector2Int startingPosition = parentChunk.GetChunkPositionVector2Int();

        int loopIndexX = 0;
        int loopIndexY = 0;

        if (parentChunk.ChunkMapFilled())
        {
            for (int x = startingPosition.x - _chunkLayerCount; x <= startingPosition.x + _chunkLayerCount; x++)
            {
                for (int y = startingPosition.y - _chunkLayerCount; y <= startingPosition.y + _chunkLayerCount; y++)
                {
                    Vector3 tilPlacementPosition = new Vector3(x, y, 0);
                    Transform tileTransform = Instantiate(_tilePrefab, tilPlacementPosition, Quaternion.identity);

                    tileTransform.SetParent(parentChunk.transform);

                    Tile tile = tileTransform.GetComponent<Tile>();

                    float chunkMapValue = parentChunk.GetChunkMapValue(loopIndexX, loopIndexY);
                    Sprite sprite = GetRandomSpriteForTile(chunkMapValue);

                    tile.SetSprite(sprite);

                    parentChunk.AddTileToChunk(tile);

                    loopIndexY++;
                }

                loopIndexX++;
                loopIndexY = 0;

                yield return new WaitForEndOfFrame();
            }
            
            parentChunk.FillChunk();

            parentChunk.FinishLoadingTiles();
        }
    }

    private Sprite GetRandomSpriteForTile(float tileType)
    {
        if (tileType >= _obstacleChance)
        {
            return _tileConfigSO.obstacleTiles[0];
        }
        else
        {
            return _tileConfigSO.groundTiles[0];
        }
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

    public float[,] GetChunkMapArrayWithPosition(Vector2Int chunkPosition)
    {
        if(!ChunkExistsAtPosition(chunkPosition))
        {
            return null;
        }

        return _chunkDictionary[chunkPosition].GetChunkMapArray();
    }

    public bool ChunkExistsAtPosition(Vector2Int chunkPosition) => _chunkDictionary.ContainsKey(chunkPosition);
}
