using UnityEngine;

public abstract class InteractableBuilding : Building, IInteractable
{
    [Header("Config")]
    [SerializeField] protected Sprite _uninteractedSprite;
    [SerializeField] protected Sprite _interactedSprite;
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    [Header("Debug Fields")]
    [SerializeField] protected Color _interactColor;
    [SerializeField] protected Color _uninteractColor;
    [SerializeField] public bool IsInteracting { get; protected set; } = false;

    protected virtual void Awake()
    {
        _uninteractColor = new Color(1, 1, 1);
        _uninteractColor.a = 1f;

        _interactColor = new Color(0, 1, 0);
        _interactColor.a = .8f;
    }

    public abstract void Interact(GameObject initiator);

    public abstract void StopInteraction();

    public void HighlightInteractable()
    {
        Debug.Log($"Highlight {gameObject.name}");
        _spriteRenderer.color = _interactColor;
    }

    public void UnhighlightInteractable()
    {
        Debug.Log($"Unhighlight {gameObject.name}");
        _spriteRenderer.color = _uninteractColor;
    }
}
