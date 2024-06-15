using UnityEngine;
using System.Collections.Generic;

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

        GenerateChunk(new Vector2Int(0, 0));
    }

    public void GenerateChunk(Vector2Int startingPosition)
    {
        Vector3 chunkPosition = new Vector3(startingPosition.x, startingPosition.y, 0);
        Transform chunkTransform = Instantiate(_chunkPrefab, chunkPosition, Quaternion.identity);

        chunkTransform.SetParent(_chunkParent);

        Chunk chunk = chunkTransform.GetComponent<Chunk>();

        _chunkDictionary.Add(startingPosition, chunk);

        for (int x = startingPosition.x - _chunkLayerCount; x <= startingPosition.x + _chunkLayerCount; x++)
        {
            for (int y = startingPosition.y - _chunkLayerCount; y <= startingPosition.y + _chunkLayerCount; y++)
            {
                Vector3 tilPlacementPosition = new Vector3(x, y, 0);
                Transform tileTransform = Instantiate(_tilePrefab, tilPlacementPosition, Quaternion.identity, chunkTransform);

                Tile tile = tileTransform.GetComponent<Tile>();

                Sprite sprite = GetRandomSpriteForTile();

                tile.SetSprite(sprite);
            }
        }
    }

    private Sprite GetRandomSpriteForTile()
    {
        return _tileConfigSO.tileSpriteList[Random.Range(0, _tileConfigSO.tileSpriteList.Count)];
    }
}
