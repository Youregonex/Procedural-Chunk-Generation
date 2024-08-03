using UnityEngine;
using System;

public class PlayerItemSelection : AgentMonobehaviourComponent
{
    public event Action<ItemDataSO> OnCurrentItemChanged;

    [Header("Debug Field")]
    [SerializeField] private InventorySlot _currentInventorySlot;

    private void Start()
    {
        HotbarDisplay.OnHotbarSlotSelected += HotbarDisplay_OnHotbarSlotSelected;    
    }

    private void OnDestroy()
    {
        DeselectCurrentSlot();
        HotbarDisplay.OnHotbarSlotSelected -= HotbarDisplay_OnHotbarSlotSelected;
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void HotbarDisplay_OnHotbarSlotSelected(InventorySlot inventorySlot)
    {
        if (_currentInventorySlot != null)
            DeselectCurrentSlot();

        ChangeCurrentInventorySlot(inventorySlot);
    }

    private void DeselectCurrentSlot()
    {
        if (_currentInventorySlot.ItemDataSO == null)
            return;

        switch (_currentInventorySlot.ItemDataSO.ItemType)
        {
            case EItemType.ActionItem:

                ActionItemDataSO actionItemDataSO = _currentInventorySlot.ItemDataSO as ActionItemDataSO;
                actionItemDataSO.OnActionItemUsed -= ActionItemDataSO_OnActionItemUsed;

                break;

            case EItemType.BuildingItem:

                BuildingItemDataSO buildingItemDataSO = _currentInventorySlot.ItemDataSO as BuildingItemDataSO;
                buildingItemDataSO.OnBuildingItemPlaced -= BuildingItemDataSO_OnBuildingItemPlaced;

                break;

            default:
                break;
        }
    }

    private void ChangeCurrentInventorySlot(InventorySlot inventorySlot)
    {
        _currentInventorySlot = inventorySlot;

        OnCurrentItemChanged?.Invoke(inventorySlot.ItemDataSO);

        if (inventorySlot.ItemDataSO == null)
            return;

        switch (inventorySlot.ItemDataSO.ItemType)
        {
            case EItemType.ActionItem:

                ActionItemDataSO actionItemDataSO = inventorySlot.ItemDataSO as ActionItemDataSO;
                actionItemDataSO.OnActionItemUsed += ActionItemDataSO_OnActionItemUsed;

                break;

            case EItemType.BuildingItem:

                BuildingItemDataSO buildingItemDataSO = inventorySlot.ItemDataSO as BuildingItemDataSO;
                buildingItemDataSO.OnBuildingItemPlaced += BuildingItemDataSO_OnBuildingItemPlaced;

                break;

            default:
                break;
        }
    }

    private void BuildingItemDataSO_OnBuildingItemPlaced()
    {
        if(_currentInventorySlot.CurrentStackSize == 1)
        {
            BuildingItemDataSO buildingItemDataSO = _currentInventorySlot.ItemDataSO as BuildingItemDataSO;
            buildingItemDataSO.OnBuildingItemPlaced -= BuildingItemDataSO_OnBuildingItemPlaced;
        }

        _currentInventorySlot.RemoveFromStackSize(1);
    }

    private void ActionItemDataSO_OnActionItemUsed()
    {
        _currentInventorySlot.RemoveFromStackSize(1);
    }
}
