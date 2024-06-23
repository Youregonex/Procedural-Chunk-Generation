using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class Inventory
{
    public event EventHandler OnInventorySlotChanged;

    [SerializeField] private List<InventorySlot> _inventoryContentList;

    private int _inventorySize;

    private bool _isInitialized = false;

    public void InitializeInventory(int inventorySize)
    {
        if (_isInitialized)
            return;

        _inventoryContentList = new List<InventorySlot>(_inventorySize);

        _inventorySize = inventorySize;

        for (int i = 0; i < _inventorySize; i++)
        {
            _inventoryContentList.Add(new InventorySlot());
        }
    }

    public void AddItemToInventory(Item itemToAdd)
    {
        if(!InventoryHasFreeSlot())
        {
            return;
        }

        InventorySlot freeSlot = GetFirstFreeSlot();

        freeSlot.SetSlotData(itemToAdd.GetItemDataSO(), itemToAdd.GetItemQuantity());

        OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    private InventorySlot GetFirstFreeSlot()
    {
        InventorySlot slot = _inventoryContentList.First(slot => slot.GetSlotItemDataSO() == null);

        return slot;
    }

    private bool InventoryHasFreeSlot() => _inventoryContentList.Any(slot => slot.GetSlotItemDataSO() == null);
    public int GetInventorySize() => _inventoryContentList.Count;
    public List<InventorySlot> GetInventoryList() => _inventoryContentList;
}
