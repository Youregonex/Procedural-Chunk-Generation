using UnityEngine;

public class Chest : InteractableBuilding
{
    [Header("Config")]
    [SerializeField] private int _containerSize;

    [Header("Debug Fields")]
    [SerializeField] private Inventory _containerInventory;
    [SerializeField] private DynamicInventoryDisplay _customContaineerDisplay;

    protected override void Awake()
    {
        base.Awake();

        _containerInventory = new Inventory();
        _containerInventory.InitializeInventory(_containerSize);
    }

    private void Start()
    {
        _customContaineerDisplay = UIComponentProvider.Instance.CustomContainerDisplay;
    }

    public override void Interact(GameObject initiator)
    {
        if (IsInteracting)
        {
            StopInteraction();
            return;
        }

        IsInteracting = true;
        _customContaineerDisplay.ShowInventory(_containerInventory);
        _spriteRenderer.sprite = _interactedSprite;
    }

    public override void StopInteraction()
    {
        IsInteracting = false;
        _customContaineerDisplay.HideInventory();
        _spriteRenderer.sprite = _uninteractedSprite;
    }
}
