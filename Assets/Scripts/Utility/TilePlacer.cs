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

    public void SetTileAtPosition(Tile tile, Vector2 tileWorldPosition, ETileType tileType)
    {
        switch(tileType)
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

    public void ClearTileAtPosition(Vector2 tileWorldPosition, ETileType tileType)
    {

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
