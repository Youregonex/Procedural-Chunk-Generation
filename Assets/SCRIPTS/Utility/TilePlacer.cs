using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;

public class TilePlacer : MonoBehaviour
{
    public static TilePlacer Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _obstacleTilemap;

    [Header("Debug Fields")]
    [SerializeField] private List<Chunk> _loadingChunks = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    //public void LoadChunkTiles(Chunk chunk, List<TileData> tileDataList)
    //{
    //    if (_loadingChunks.Contains(chunk))
    //        return;

    //    StartCoroutine(LoadChunkTilesCoroutine(chunk, tileDataList));
    //}

    //private IEnumerator LoadChunkTilesCoroutine(Chunk chunk, List<TileData> tileDataList)
    //{
    //    _loadingChunks.Add(chunk);

    //    int a = 10;
    //    int b = 0;

    //    for (int i = 0; i < tileDataList.Count; i++)
    //    {
    //        SetTile(tileDataList[i]);
    //        b++;

    //        if(b == a)
    //        {
    //            b = 0;
    //            yield return new WaitForEndOfFrame();
    //        }
    //    }

    //    _loadingChunks.Remove(chunk);
    //}

    public void UnloadChunkTiles(List<TileData> tileDataList)
    {
        for (int i = 0; i < tileDataList.Count; i++)
        {
            ClearTile(tileDataList[i]);
        }
    }

    public void SetTile(TileData tileData)
    {
        if (_groundTilemap == null || _obstacleTilemap == null)
        {
            UnityEngine.Debug.LogError("Tilemap is missing!");
            return;
        }

        Vector2 tileWorldPosition = new Vector2(tileData.position.x, tileData.position.y);

        switch (tileData.type)
        {
            default:
            case ETileType.Ground:

                Vector3Int tilemapPosition = _groundTilemap.WorldToCell(tileWorldPosition);
                _groundTilemap.SetTile(tilemapPosition, tileData.tile);

                break;

            case ETileType.Obstacle:

                tilemapPosition = _obstacleTilemap.WorldToCell(tileWorldPosition);
                _obstacleTilemap.SetTile(tilemapPosition, tileData.tile);

                break;
        }
    }

    public bool HasObstacleAtPosition(Vector2 position)
    {
        Vector3Int tilemapPosition = _obstacleTilemap.WorldToCell(position);

        return _obstacleTilemap.HasTile(tilemapPosition) ? true : false;
    }

    public void ClearTile(TileData tileData)
    {
        if (_groundTilemap == null || _obstacleTilemap == null)
            return;

        Vector2 tileWorldPosition = new Vector2(tileData.position.x, tileData.position.y);

        switch (tileData.type)
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
