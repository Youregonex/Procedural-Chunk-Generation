using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileData
{
    public TileBase tile;
    public Vector2Int position;
    public ETileType type;

    public TileData(TileBase tile, Vector2Int position, ETileType type)
    {
        this.tile = tile;
        this.position = position;
        this.type = type;
    }
}
