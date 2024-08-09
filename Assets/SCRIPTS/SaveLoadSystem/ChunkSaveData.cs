using UnityEngine;

[System.Serializable]
public class ChunkSaveData : SaveData
{
    public Vector2Int position;

    public SerializableDictionary<Vector2Int, ResourceNode> nodePositionMapDictionary;

    public ChunkSaveData(Vector2Int position,
                         SerializableDictionary<Vector2Int, ResourceNode> nodePositionMapDictionary)
    {
        this.position = position;
        this.nodePositionMapDictionary = nodePositionMapDictionary;
    }
}
