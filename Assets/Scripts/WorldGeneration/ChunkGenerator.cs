using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ChunkGenerator : MonoBehaviour
{
    private Dictionary<Vector2Int, Chunk> _chunkDictionary;
    private List<Chunk> _chunkList;
    private List<Chunk> _loadedChunkList;
    private List<Chunk> _unloadedChunkList;

    [Header("Tile Setup")]
    [SerializeField] private TileConfigSO _tileConfigSO;
    [SerializeField] private Transform _tilePrefab;

    [Header("Chunk Setup")]
    [SerializeField] private Transform _chunkParent;
    [SerializeField] private Transform _chunkPrefab;
    [SerializeField] private int _chunkLayerCount;

    private void Awake()
    {
        _chunkDictionary = new Dictionary<Vector2Int, Chunk>();
        _chunkList = new List<Chunk>();
    }

    private void Start()
    {
        Chunk.OnPlayerEnteredChunk += Chunk_OnPlayerEnteredChunk;
        Chunk.OnPlayerLeftChunk += Chunk_OnPlayerLeftChunk;

        CreateInitialChunks();
    }

    private void CreateInitialChunks()
    {
        int distanceToNextChunk = (_chunkLayerCount * 2) + 1;

        StartCoroutine(GenerateChunk(new Vector2Int(-distanceToNextChunk, distanceToNextChunk)));
        StartCoroutine(GenerateChunk(new Vector2Int(-distanceToNextChunk, -distanceToNextChunk)));
        StartCoroutine(GenerateChunk(new Vector2Int(-distanceToNextChunk, 0)));
        StartCoroutine(GenerateChunk(new Vector2Int(0, distanceToNextChunk)));
        StartCoroutine(GenerateChunk(new Vector2Int(0, 0)));
        StartCoroutine(GenerateChunk(new Vector2Int(0, -distanceToNextChunk)));
        StartCoroutine(GenerateChunk(new Vector2Int(distanceToNextChunk, 0)));
        StartCoroutine(GenerateChunk(new Vector2Int(distanceToNextChunk, distanceToNextChunk)));
        StartCoroutine(GenerateChunk(new Vector2Int(distanceToNextChunk, -distanceToNextChunk)));
    }

    private void Chunk_OnPlayerEnteredChunk(object sender, Chunk.OnPlayerCrossedChunkeventArgs e)
    {
        Chunk chunk = sender as Chunk;

        List<Vector2Int> chunkNeighbours = chunk.GetNeighbourChunkList();

        foreach (Vector2Int neighbourPosition in chunkNeighbours)
        {
            if (_chunkDictionary.ContainsKey(neighbourPosition))
                continue;

            StartCoroutine(GenerateChunk(neighbourPosition));
        }
    }

    private void Chunk_OnPlayerLeftChunk(object sender, Chunk.OnPlayerCrossedChunkeventArgs e)
    {

    }

    public IEnumerator GenerateChunk(Vector2Int startingPosition)
    {
        if (_chunkDictionary.ContainsKey(startingPosition))
        {
            Debug.LogError($"Chunk at {startingPosition} is already created!");
            yield break;
        }

        Vector3 chunkPosition = new Vector3(startingPosition.x, startingPosition.y, 0);
        Transform chunkTransform = Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);

        chunkTransform.gameObject.name = $"Chunk  ({startingPosition.x} | {startingPosition.y})";

        chunkTransform.SetParent(_chunkParent);

        Chunk chunk = chunkTransform.GetComponent<Chunk>();
        chunk.InitializeChunk(_chunkLayerCount);

        _chunkDictionary.Add(startingPosition, chunk);
        _chunkList.Add(chunk);

        for (int y = startingPosition.y + _chunkLayerCount; y >= startingPosition.y - _chunkLayerCount; y--)
        {
            for (int x = startingPosition.x - _chunkLayerCount; x <= startingPosition.x + _chunkLayerCount; x++)
            {
                Vector3 tilPlacementPosition = new Vector3(x, y, 0);

                Transform tileTransform = Instantiate(_tilePrefab, tilPlacementPosition, Quaternion.identity);

                tileTransform.SetParent(chunkTransform);

                Tile tile = tileTransform.GetComponent<Tile>();

                Sprite sprite = GetRandomSpriteForTile();

                tile.SetSprite(sprite);

                chunk.AddTileToChunk(tile);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private Sprite GetRandomSpriteForTile()
    {
        return _tileConfigSO.tileSpriteList[UnityEngine.Random.Range(0, _tileConfigSO.tileSpriteList.Count)];
    }
}
