using UnityEngine;

[System.Serializable]
public class ContainerBuilding : InteractableBuilding
{
    [Header("Config")]
    [SerializeField] private int _containerSize;

    [Header("Debug Fields")]
    [SerializeField] private Inventory _containerInventory;
    [SerializeField] private DynamicInventoryDisplay _customContainerDisplay;

    public override void Initialize(BuildingItemDataSO buildingItemDataSO)
    {
        base.Initialize(buildingItemDataSO);

        _customContainerDisplay = UIComponentProvider.Instance.CustomContainerDisplay;
        _containerInventory = new Inventory();
        _containerInventory.InitializeInventory(_containerSize);
    }

    public override SaveData GenerateSaveData()
    {
        ContainerSaveData chestSaveData = new ContainerSaveData(_buildingItemDataSO, transform.position, _containerInventory);

        return chestSaveData;
    }

    public override void LoadFromSaveData(SaveData saveData)
    {
        base.LoadFromSaveData(saveData);

        ContainerSaveData containerSaveData = saveData as ContainerSaveData;

        _containerInventory = containerSaveData.containerInventory;
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
