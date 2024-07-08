using UnityEngine;

public class Chest : InteractableBuilding
{
    [Header("Config")]
    [SerializeField] private int _containerSize;

    [Header("Debug Fields")]
    [SerializeField] private Inventory _containerInventory;
    [SerializeField] private DynamicInventoryDisplay _customContainerDisplay;

    protected override void Awake()
    {
        base.Awake();

        _containerInventory = new Inventory();
        _containerInventory.InitializeInventory(_containerSize);
    }

    private void Start()
    {
        _customContainerDisplay = UIComponentProvider.Instance.CustomContainerDisplay;
    }

    public override void Interact(GameObject initiator)
    {
        if (IsInteracting)
        {
            StopInteraction();
            return;
        }

        IsInteracting = true;
        _customContainerDisplay.ShowInventory(_containerInventory);
        _spriteRenderer.sprite = _interactedSprite;
    }

    public override void StopInteraction()
    {
        IsInteracting = false;
        _customContainerDisplay.HideInventory();
        _spriteRenderer.sprite = _uninteractedSprite;
    }
}
