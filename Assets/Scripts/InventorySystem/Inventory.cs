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

    public int AddItemToInventory(ItemDataSO itemDataSO, int amount)
    {

        if (ContainsItem(itemDataSO, out List<InventorySlot> sameItemSlots)) // If inventory cointains same item -> get List of slots, containing this item
        {
            foreach (InventorySlot slot in sameItemSlots)
            {
                if (slot.ItemDataSO.MaxStackSize - slot.CurrentStackSize >= amount) // If we can add whole item quantity to already existing stack -> update slot item quantity
                {
                    slot.AddToStackSize(amount);
                    OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                    return 0;
                }
                else // If we can't add whole item quantity -> add what we can, reduce quantity
                {
                    int slotCanAdd = slot.ItemDataSO.MaxStackSize - slot.CurrentStackSize;

                    if (slotCanAdd == 0)
                        continue;

                    slot.AddToStackSize(slotCanAdd);

                    amount -= slotCanAdd;

                    OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        while (amount > 0 && InventoryHasFreeSlot(out InventorySlot freeSlot)) // If we didnt find slots with same item/all slots with same item are full -> add item to free slot
        {
            if (itemDataSO.MaxStackSize >= amount) // If we can add whole item quantity to one slot -> update slot
            {
                freeSlot.SetSlotData(itemDataSO, amount);

                OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

                return 0;
            }
            else
            {
                freeSlot.SetSlotData(itemDataSO, itemDataSO.MaxStackSize); // Add amount that slot can hold
                amount -= itemDataSO.MaxStackSize;

                OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        return amount;
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
