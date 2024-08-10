using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilePlacer : MonoBehaviour
{
    public static TilePlacer Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _obstacleTilemap;
    [SerializeField] private Dictionary<Chunk, List<TileData>> _tileDictionary = new Dictionary<Chunk, List<TileData>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public void LoadChunkTiles(Chunk chunk)
    {
        if(!_tileDictionary.ContainsKey(chunk))
        {
            List<TileData> tileDataMapList = ChunkGenerator.Instance.ChunkNoiseMapToTileData(chunk);
            _tileDictionary.Add(chunk, tileDataMapList);

            for (int i = 0; i < tileDataMapList.Count; i++)
            {
                SetTile(tileDataMapList[i]);
            }
        }
        else
        {
            for (int i = 0; i < _tileDictionary[chunk].Count; i++)
            {
                SetTile(_tileDictionary[chunk][i]);
            }
        }
    }

    public void UnloadChunkTiles(Chunk chunk)
    {
        List<TileData> tileDataMapList = ChunkGenerator.Instance.ChunkNoiseMapToTileData(chunk);

        for (int i = 0; i < tileDataMapList.Count; i++)
        {
            ClearTileAtPosition(tileDataMapList[i]);
        }
    }

    public void SetTile(TileData tileData)
    {
        if (_groundTilemap == null || _obstacleTilemap == null)
        {
            UnityEngine.Debug.LogError("Tilemap is missing!");
            return;
        }

        Vector2 tileWorldPosition = new Vector2(tileData.position.x, tileData.position.y);

        switch (tileData.type)
        {
            default:
            case ETileType.Ground:

                Vector3Int tilemapPosition = _groundTilemap.WorldToCell(tileWorldPosition);
                _groundTilemap.SetTile(tilemapPosition, tileData.tile);

                break;

            case ETileType.Obstacle:

                tilemapPosition = _obstacleTilemap.WorldToCell(tileWorldPosition);
                _obstacleTilemap.SetTile(tilemapPosition, tileData.tile);

                break;
        }
    }

    public bool HasObstacleAtPosition(Vector2 position)
    {
        Vector3Int tilemapPosition = _obstacleTilemap.WorldToCell(position);

        return _obstacleTilemap.HasTile(tilemapPosition) ? true : false;
    }

    public void ClearTileAtPosition(TileData tileData)
    {
        if (_groundTilemap == null || _obstacleTilemap == null)
            return;

        Vector2 tileWorldPosition = new Vector2(tileData.position.x, tileData.position.y);

        switch (tileData.type)
        {
            default:
            case ETileType.Ground:

                Vector3Int tilemapPosition = _groundTilemap.WorldToCell(tileWorldPosition);

                if(_groundTilemap.GetTile(tilemapPosition))
                {
                    _groundTilemap.SetTile(tilemapPosition, null);
                }

                break;

            case ETileType.Obstacle:

                tilemapPosition = _obstacleTilemap.WorldToCell(tileWorldPosition);

                if (_obstacleTilemap.GetTile(tilemapPosition))
                {
                    _obstacleTilemap.SetTile(tilemapPosition, null);
                }

                break;
        }
    }
}
