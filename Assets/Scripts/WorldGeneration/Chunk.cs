using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class Chunk : MonoBehaviour
{
    private List<Tile> _chunkTiles;
    private BoxCollider2D _trigerZone;

    public static event EventHandler<OnPlayerCrossedChunkeventArgs> OnPlayerEnteredChunk;
    public static event EventHandler<OnPlayerCrossedChunkeventArgs> OnPlayerLeftChunk;
    public class OnPlayerCrossedChunkeventArgs : EventArgs
    {
        public Vector2 playerPosition;
    }

    [Header("Debug Fields")]
    [SerializeField] private List<Vector2Int> _neighbourChunkList;
    [SerializeField] private int _chunkLayerCount;
    [SerializeField] private Transform _markerPrefab;
    [SerializeField] private bool _isLoaded;

    private void Awake()
    {
        _chunkTiles = new List<Tile>();
        _trigerZone = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IPlayer>() != null)
        {
            OnPlayerEnteredChunk?.Invoke(this, new OnPlayerCrossedChunkeventArgs
            {
                playerPosition = collision.transform.position
            });
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision is IPlayer)
        {
            OnPlayerLeftChunk?.Invoke(this, new OnPlayerCrossedChunkeventArgs
            {
                playerPosition = collision.transform.position
            });
        }
    }

    public void AddTileToChunk(Tile tile)
    {
        _chunkTiles.Add(tile);
    }

    public void SetChunkSize(int chunkLayerCount)
    {
        _chunkLayerCount = chunkLayerCount;
    }

    public void InitializeChunk(int chunkLayerCount)
    {
        _chunkLayerCount = chunkLayerCount;

        int distanceToNextChunk = (_chunkLayerCount * 2) + 1;

        for (int x = (int)transform.position.x + -distanceToNextChunk; x <= (int)transform.position.x + distanceToNextChunk; x += distanceToNextChunk)
        {
            for (int y = (int)transform.position.y + -distanceToNextChunk; y <= (int)transform.position.y + distanceToNextChunk; y += distanceToNextChunk)
            {
                if (new Vector3(x, y) == transform.position)
                    continue;

                Vector2Int neighbourChunkPosition = new Vector2Int(x, y);

                _neighbourChunkList.Add(neighbourChunkPosition);

                Transform marker = Instantiate(_markerPrefab, new Vector3(x, y), Quaternion.identity);
                marker.SetParent(gameObject.transform);

                float trigerZoneSize = (_chunkLayerCount * 2) + 1 - .01f;

                _trigerZone.size = new Vector2(trigerZoneSize, trigerZoneSize);
            }
        }
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
            tile.gameObject.SetActive(false);
        }
    }

    public bool IsLoaded() => _isLoaded;

    public List<Vector2Int> GetNeighbourChunkList() => _neighbourChunkList;

    public Vector2Int GetRandomNeighbourChunk()
    {
        return _neighbourChunkList[UnityEngine.Random.Range(0, _neighbourChunkList.Count)];
    }
}
