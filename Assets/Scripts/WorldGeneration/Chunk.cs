using UnityEngine;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    private List<Tile> _chunkTiles;

    private void Awake()
    {
        _chunkTiles = new List<Tile>();
    }

    public void AddTileToChunk(Tile tile)
    {
        _chunkTiles.Add(tile);
    }
}
