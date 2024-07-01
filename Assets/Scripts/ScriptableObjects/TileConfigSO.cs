using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tile Config")]
public class TileConfigSO : ScriptableObject
{
    public List<Tile> groundTiles;
    public List<TileBase> obstacleTiles;
}
