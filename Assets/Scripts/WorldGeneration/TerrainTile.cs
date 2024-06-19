using UnityEngine;

[SelectionBase]
public class TerrainTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Chunk _parentChunk;

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void SetParentChunk(Chunk chunk) => _parentChunk = chunk;
}
