using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacer : MonoBehaviour
{
    public static TilePlacer Instance { get; private set; }

    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _obstacleTilemap;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public void SetTileAtPosition(TileBase tile, Vector2 tileWorldPosition, ETileType tileType)
    {
        if (_groundTilemap == null || _obstacleTilemap == null)
            return;

        switch (tileType)
        {
            default:
            case ETileType.Ground:

                Vector3Int tilemapPosition = _groundTilemap.WorldToCell(tileWorldPosition);
                _groundTilemap.SetTile(tilemapPosition, tile);

                break;

            case ETileType.Obstacle:

                tilemapPosition = _obstacleTilemap.WorldToCell(tileWorldPosition);
                _obstacleTilemap.SetTile(tilemapPosition, tile);

                break;
        }
    }

    public bool HasObstaclAtPosition(Vector2 position)
    {
        Vector3Int tilemapPosition = _obstacleTilemap.WorldToCell(position);

        return _obstacleTilemap.HasTile(tilemapPosition) ? true : false;
    }

    public void ClearTileAtPosition(Vector2 tileWorldPosition, ETileType tileType)
    {
        if (_groundTilemap == null || _obstacleTilemap == null)
            return;

        switch (tileType)
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
