using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Config/Tile Config")]
public class TileConfigSO : ScriptableObject
{
    public List<Tile> groundTiles;
    public List<TileBase> obstacleTiles;
}
