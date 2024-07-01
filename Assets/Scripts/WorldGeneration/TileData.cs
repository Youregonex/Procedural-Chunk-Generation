using UnityEngine;
using UnityEngine.Tilemaps;

public struct TileData
{
    public TileData(TileBase tile, Vector2Int tilePosition, ETileType tileType)
    {
        Tile = tile;
        TilePosition = tilePosition;
        TileType = tileType;
    }

    public TileBase Tile;
    public Vector2Int TilePosition;
    public ETileType TileType;
}
