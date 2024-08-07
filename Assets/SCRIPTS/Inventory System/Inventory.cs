using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class Inventory
{
    public event EventHandler Inventory_OnInventorySlotChanged;

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
            InventorySlot inventorySlot = new InventorySlot();

            _inventoryContentList.Add(inventorySlot);

            inventorySlot.InventorySlot_OnInventorySlotChanged += InventorySlot_OnInventorySlotChanged;
        }
    }

    public int AddItemToInventory(ItemDataSO itemDataSO, int amount)
    {
        if (ContainsItem(itemDataSO, out List<InventorySlot> sameItemSlots)) // If inventory cointains same item -> get List of slots, containing this item
        {
            foreach (InventorySlot slot in sameItemSlots)
            {
                if (slot.ItemDataSO.MaxStackSize - slot.CurrentStackSize >= amount) // If we can add whole item quantity to already existing stack -> update slot item quantity
                {
                    slot.AddToStackSize(amount);
                    Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                    return 0;
                }
                else // If we can't add whole item quantity -> add what we can, reduce quantity
                {
                    int slotCanAdd = slot.ItemDataSO.MaxStackSize - slot.CurrentStackSize;

                    if (slotCanAdd == 0)
                        continue;

                    slot.AddToStackSize(slotCanAdd);

                    amount -= slotCanAdd;

                    Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        while (amount > 0 && InventoryHasFreeSlot(out InventorySlot freeSlot)) // If we didnt find slots with same item/all slots with same item are full -> add item to free slot
        {
            if (itemDataSO.MaxStackSize >= amount) // If we can add whole item quantity to one slot -> update slot
            {
                freeSlot.SetSlotData(itemDataSO, amount);

                Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                return 0;
            }
            else
            {
                freeSlot.SetSlotData(itemDataSO, itemDataSO.MaxStackSize); // Add amount that slot can hold
                amount -= itemDataSO.MaxStackSize;

                Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        return amount;
    }

    public bool RemoveItem(ItemDataSO itemDataSO, int quantity)
    {
        if (ContainsItem(itemDataSO, out List<InventorySlot> inventorySlots))
        {
            int inventoryHasItemAmount = 0;

            foreach (InventorySlot slot in inventorySlots)
            {
                inventoryHasItemAmount += slot.CurrentStackSize;
            }

            if (inventoryHasItemAmount < quantity)
                return false;

            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.CurrentStackSize >= quantity)
                {
                    slot.RemoveFromStackSize(quantity);

                    if (slot.CurrentStackSize == 0)
                        slot.ClearSlot();

                    Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                    return true;
                }
                else
                {
                    quantity -= slot.CurrentStackSize;
                    slot.ClearSlot();

                    Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        return false;
    }

    public bool ContainsItemWithQuantity(ItemDataSO item, int amount)
    {
        if (ContainsItem(item, out List<InventorySlot> inventorySlots))
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.CurrentStackSize >= amount)
                {
                    return true;
                }
                else
                {
                    amount -= slot.CurrentStackSize;
                }
            }
        }

        return false;
    }

    private void InventorySlot_OnInventorySlotChanged(object sender, EventArgs e)
    {
        Inventory_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
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
