using UnityEngine;
using System;

[Serializable]
public class InventorySlot
{
    public event EventHandler InventorySlot_OnInventorySlotChanged;

    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _currentStackSize;

    public ItemDataSO ItemDataSO => _itemDataSO;
    public int CurrentStackSize => _currentStackSize;

    public InventorySlot()
    {
        _itemDataSO = null;
        _currentStackSize = -1;
    }

    public InventorySlot(ItemDataSO itemDataSO, int quantity)
    {
        _itemDataSO = itemDataSO;
        _currentStackSize = quantity;
    }

    public void SetSlotData(ItemDataSO itemDataSO, int quantity)
    {
        _itemDataSO = itemDataSO;
        _currentStackSize = quantity;

        InventorySlot_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool StackCanFit(int quantityToAdd, out int stackCantFit)
    {
        int roomLeftInStack = _itemDataSO.MaxStackSize - _currentStackSize;

        if (roomLeftInStack >= quantityToAdd)
        {
            stackCantFit = 0;
            return true;
        }
        else
        {
            stackCantFit = quantityToAdd - roomLeftInStack;
            return false;
        }
    }

    public void SetMaxStackSize()
    {
        _currentStackSize = _itemDataSO.MaxStackSize;

        InventorySlot_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddToStackSize(int quantity)
    {
        _currentStackSize += quantity;

        InventorySlot_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveFromStackSize(int quantity)
    {
        _currentStackSize -= quantity;

        InventorySlot_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearSlot()
    {
        _itemDataSO = null;
        _currentStackSize = -1;

        InventorySlot_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool SplitStack(out int halfStack)
    {
        if (_currentStackSize <= 1)
        {
            halfStack = 0;
            return false;
        }

        halfStack = Mathf.RoundToInt(_currentStackSize / 2);
        RemoveFromStackSize(halfStack);

        InventorySlot_OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    public bool SlotIsFull() => _currentStackSize == _itemDataSO.MaxStackSize;
}
