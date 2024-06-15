using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ChunkGenerator : MonoBehaviour
{
    private Dictionary<Vector2Int, Chunk> _chunkDictionary;

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
    }

    private void Start()
    {
        Chunk.OnPlayerEnteredChunkRange += Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange += Chunk_OnPlayerLeftChunkRange;

        CreateInitialChunks();
    }

    private void CreateInitialChunks()
    {
        int distanceToNextChunk = (_chunkLayerCount * 2) + 1;

        GenerateChunk(new Vector2Int(-distanceToNextChunk, -distanceToNextChunk));
        GenerateChunk(new Vector2Int(-distanceToNextChunk, distanceToNextChunk));
        GenerateChunk(new Vector2Int(-distanceToNextChunk, 0));
        GenerateChunk(new Vector2Int(0, -distanceToNextChunk));
        GenerateChunk(new Vector2Int(0, 0));
        GenerateChunk(new Vector2Int(0, distanceToNextChunk));
        GenerateChunk(new Vector2Int(distanceToNextChunk, 0));
        GenerateChunk(new Vector2Int(distanceToNextChunk, distanceToNextChunk));
        GenerateChunk(new Vector2Int(distanceToNextChunk, -distanceToNextChunk));
    }

    private void Chunk_OnPlayerEnteredChunkRange(object sender, EventArgs e)
    {
        Chunk enteredChunk = sender as Chunk;

        if(!enteredChunk.IsFilled())
        {
            StartCoroutine(FillChunkWithTilesCoroutine(enteredChunk));
        }

        enteredChunk.LoadChunk();

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
            exitChunk.OnFinishTileLoading += ExitChunk_OnFinishTileLoading;
        }
        else
        {
            exitChunk.UnloadChunk();
        }
    }

    private void ExitChunk_OnFinishTileLoading(object sender, Chunk.OnFinishTileLoadingEventArgs e)
    {
        Chunk chunk = e.chunk;

        if(!chunk.IsPlayerInRange())
            chunk.UnloadChunk();

        chunk.OnFinishTileLoading -= ExitChunk_OnFinishTileLoading;
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

        return chunk;
    }

    private IEnumerator FillChunkWithTilesCoroutine(Chunk parentChunk)
    {
        parentChunk.StartLoadingTiles();

        Vector2Int startingPosition = parentChunk.GetChunkPositionVector2Int();

        for (int y = startingPosition.y + _chunkLayerCount; y >= startingPosition.y - _chunkLayerCount; y--)
        {
            for (int x = startingPosition.x - _chunkLayerCount; x <= startingPosition.x + _chunkLayerCount; x++)
            {
                Vector3 tilPlacementPosition = new Vector3(x, y, 0);
                Transform tileTransform = Instantiate(_tilePrefab, tilPlacementPosition, Quaternion.identity);

                tileTransform.SetParent(parentChunk.transform);

                Tile tile = tileTransform.GetComponent<Tile>();

                Sprite sprite = GetRandomSpriteForTile();

                tile.SetSprite(sprite);

                parentChunk.AddTileToChunk(tile);
            }

            yield return new WaitForEndOfFrame();
        }

        parentChunk.FillChunk();

        parentChunk.FinishLoadingTiles();
    }

    private Sprite GetRandomSpriteForTile()
    {
        return _tileConfigSO.tileSpriteList[UnityEngine.Random.Range(0, _tileConfigSO.tileSpriteList.Count)];
    }

    private void OnDestroy()
    {
        Chunk.OnPlayerEnteredChunkRange -= Chunk_OnPlayerEnteredChunkRange;
        Chunk.OnPlayerLeftChunkRange -= Chunk_OnPlayerLeftChunkRange;
    }
}
