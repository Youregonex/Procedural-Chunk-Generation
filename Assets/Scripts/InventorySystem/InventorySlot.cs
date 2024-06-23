using UnityEngine;
using System;

[Serializable]
public class InventorySlot
{
    public event EventHandler OnInventorySlotChanged;

    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _currentStackSize;

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

        OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddToStackSize(int quantity)
    {
        _currentStackSize += quantity;

        OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SubstractFromStackSize(int quantity)
    {
        _currentStackSize -= quantity;

        OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearSlot()
    {
        _itemDataSO = null;
        _currentStackSize = -1;

        OnInventorySlotChanged?.Invoke(this, EventArgs.Empty);
    }

    public ItemDataSO GetSlotItemDataSO() => _itemDataSO;
    public int GetCurrentStackSize() => _currentStackSize;
}
