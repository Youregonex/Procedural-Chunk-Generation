using UnityEngine;
using System;
using Youregone.Utilities;

public class PlayerItemSelection : AgentNetworkBehaviourComponent
{
    public event Action<ItemDataSO> OnCurrentItemChanged;

    [Header("Debug Field")]
    [SerializeField] private InventorySlot _currentInventorySlot;
    [SerializeField] private PlayerInput _playerInput;

    public override void Initialize()
    {
        GetAgentCore();

        _playerInput = AgentCore.GetAgentComponent<PlayerInput>();

        if (!IsOwner)
            return;

        HotbarDisplay.OnHotbarSlotSelected += HotbarDisplay_OnHotbarSlotSelected;
        _playerInput.OnMouseSecondary += PlayerInput_OnMouseSecondary;
    }

    public override void OnNetworkDespawn()
    {
        DeselectCurrentSlot();

        HotbarDisplay.OnHotbarSlotSelected -= HotbarDisplay_OnHotbarSlotSelected;

        if (_playerInput != null)
            _playerInput.OnMouseSecondary -= PlayerInput_OnMouseSecondary;
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void PlayerInput_OnMouseSecondary()
    {
        if(_currentInventorySlot.ItemDataSO != null &&_currentInventorySlot.ItemDataSO.ItemType == EItemType.ActionItem && !Utility.PointerOverUIObject())
        {
            ActionItemDataSO actionItemDataSO = _currentInventorySlot.ItemDataSO as ActionItemDataSO;
            actionItemDataSO.Use(AgentCore);
        }
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

        Debug.Log($"{NetworkObject.OwnerClientId} CurrentItemChanged");
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
        if (_currentInventorySlot.CurrentStackSize == 1)
        {
            ActionItemDataSO actionItemDataSO = _currentInventorySlot.ItemDataSO as ActionItemDataSO;
            actionItemDataSO.OnActionItemUsed -= ActionItemDataSO_OnActionItemUsed;
        }
        _currentInventorySlot.RemoveFromStackSize(1);
    }
}