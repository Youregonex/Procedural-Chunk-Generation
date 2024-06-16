using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tile Config")]
public class TileConfigSO : ScriptableObject
{
    public List<Sprite> groundTiles;
    public List<Sprite> obstacleTiles;
}
