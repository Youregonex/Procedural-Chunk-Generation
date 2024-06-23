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

    public bool AddItemToInventory(Item itemToAdd)
    {
        if(InventoryHasSameSlot(itemToAdd.GetItemDataSO(), out List<InventorySlot> sameItemList))
        {
            foreach(InventorySlot slot in sameItemList)
            {
                if(slot.StackCanFit(itemToAdd.GetItemQuantity(), out int slotCantFitQuantity))
                {
                    slot.AddToStackSize(itemToAdd.GetItemQuantity());
                    itemToAdd.ChangeItemQuantity(0);

                    OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                    return true;
                }
                else
                {
                    slot.SetMaxStackSize();
                    itemToAdd.ChangeItemQuantity(slotCantFitQuantity);

                    OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        while (itemToAdd.GetItemQuantity() != 0 && InventoryHasFreeSlot())
        {
            if (itemToAdd.GetItemQuantity() <= itemToAdd.GetItemDataSO().MaxStackSize)
            {
                InventorySlot freeSlot = GetFirstFreeSlot();

                freeSlot.SetSlotData(itemToAdd.GetItemDataSO(), itemToAdd.GetItemQuantity());

                itemToAdd.ChangeItemQuantity(0);

                OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                return true;
            }
            else
            {
                InventorySlot freeSlot = GetFirstFreeSlot();

                freeSlot.SetSlotData(itemToAdd.GetItemDataSO(), itemToAdd.GetItemDataSO().MaxStackSize);

                itemToAdd.ChangeItemQuantity(itemToAdd.GetItemQuantity() - itemToAdd.GetItemDataSO().MaxStackSize);
            }
        }

        OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

        return false;
    }

    private InventorySlot GetFirstFreeSlot()
    {
        InventorySlot slot = _inventoryContentList.First(slot => slot.GetSlotItemDataSO() == null);

        return slot;
    }

    private bool InventoryHasSameSlot(ItemDataSO itemDataSO, out List<InventorySlot> sameSlotsList)
    {
        sameSlotsList = new List<InventorySlot>();

        sameSlotsList = _inventoryContentList.Where(slot => slot.GetSlotItemDataSO() == itemDataSO).ToList();

        if(sameSlotsList.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool InventoryHasFreeSlot() => _inventoryContentList.Any(slot => slot.GetSlotItemDataSO() == null);
    public int GetInventorySize() => _inventoryContentList.Count;
    public List<InventorySlot> GetInventoryList() => _inventoryContentList;
}
