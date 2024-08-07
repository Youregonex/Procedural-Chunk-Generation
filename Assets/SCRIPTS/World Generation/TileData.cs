using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileData
{
    public TileBase Tile;
    public Vector2Int TilePosition;
    public ETileType TileType;

    public TileData(TileBase tile, Vector2Int tilePosition, ETileType tileType)
    {
        Tile = tile;
        TilePosition = tilePosition;
        TileType = tileType;
    }
}
