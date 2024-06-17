using UnityEngine;

[SelectionBase]
public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Chunk _parentChunk;

    public void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    public void SetParentChunk(Chunk chunk) => _parentChunk = chunk;
}
