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

    public List<InventorySlot> InventoryContentList => _inventoryContentList; 
    public int InventorySize => _inventoryContentList.Count;

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

    public bool AddItemToInventory(Item item)
    {
        if (ContainsItem(item.ItemDataSO, out List<InventorySlot> sameItemSlots)) // If inventory cointains same item -> get List of slots, containing this item
        {
            foreach (InventorySlot slot in sameItemSlots)
            {
                if (slot.ItemDataSO.MaxStackSize - slot.CurrentStackSize >= item.ItemQuantity) // If we can add whole item quantity to already existing stack -> update slot item quantity
                {
                    slot.AddToStackSize(item.ItemQuantity);
                    item.ChangeItemQuantity(0);
                    OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                    return true;
                }
                else // If we can't add whole item quantity -> add what we can, reduce quantity
                {
                    int slotCanAdd = slot.ItemDataSO.MaxStackSize - slot.CurrentStackSize;

                    if (slotCanAdd == 0)
                        continue;

                    slot.AddToStackSize(slotCanAdd);

                    item.ChangeItemQuantity(item.ItemQuantity - slotCanAdd);

                    OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        while (item.ItemQuantity > 0 && InventoryHasFreeSlot(out InventorySlot freeSlot)) // If we didnt find slots with same item/all slots with same item are full -> add item to free slot
        {
            if (item.ItemDataSO.MaxStackSize >= item.ItemQuantity) // If we can add whole item quantity to one slot -> update slot
            {
                freeSlot.SetSlotData(item.ItemDataSO, item.ItemQuantity);

                OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                return true;
            }
            else
            {
                freeSlot.SetSlotData(item.ItemDataSO, item.ItemDataSO.MaxStackSize); // Add amount that slot can hold
                item.ChangeItemQuantity(item.ItemQuantity - item.ItemDataSO.MaxStackSize);

                OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        return false;
    }

    private bool ContainsItem(ItemDataSO itemDataSO, out List<InventorySlot> sameSlotsList)
    {
        sameSlotsList = new List<InventorySlot>();

        sameSlotsList = _inventoryContentList.Where(slot => slot.ItemDataSO == itemDataSO).ToList();

        if(sameSlotsList.Count != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool InventoryHasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = _inventoryContentList.FirstOrDefault(slot => slot.ItemDataSO == null);

        if (freeSlot == null)
            return false;

        return true;
    }
}
