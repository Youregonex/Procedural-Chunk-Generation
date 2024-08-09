using UnityEngine;

public abstract class InteractableBuilding : Building, IInteractable
{
    [Header("Config")]
    [SerializeField] protected Sprite _uninteractedSprite;
    [SerializeField] protected Sprite _interactedSprite;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected bool _saveInitialColor;

    [Header("Debug Fields")]
    [SerializeField] protected Color _interactColor;
    [SerializeField] protected Color _uninteractColor;
    [SerializeField] public bool IsInteracting { get; protected set; } = false;


    public override void Initialize(BuildingItemDataSO buildingItemDataSO)
    {
        base.Initialize(buildingItemDataSO);

        if (!_saveInitialColor)
        {
            _uninteractColor = new Color(1, 1, 1);
            _uninteractColor.a = 1f;
        }

        _interactColor = new Color(0, 1, 0);
        _interactColor.a = .8f;
    }

    public abstract void Interact(GameObject initiator);

    public abstract void StopInteraction();

    public void HighlightInteractable()
    {
        _spriteRenderer.color = _interactColor;
    }

    public void UnhighlightInteractable()
    {
        _spriteRenderer.color = _uninteractColor;
    }
}
