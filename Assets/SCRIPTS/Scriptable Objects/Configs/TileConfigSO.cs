using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Configs/Tile Config")]
public class TileConfigSO : ScriptableObject
{
    [field: SerializeField] public List<Tile> GroundTiles { get; private set; } // First element has higher chance of spawning
    [field: SerializeField] public List<TileBase> ObstacleTiles { get; private set; }
}
