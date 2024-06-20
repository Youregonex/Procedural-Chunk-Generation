using UnityEngine;

[System.Serializable]
public class InventorySlot
{
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
    }

    public void AddToStackSize(int quantity)
    {
        _currentStackSize += quantity;
    }

    public void SubstractFromStackSize(int quantity)
    {
        _currentStackSize -= quantity;
    }

    public ItemDataSO GetSlotItemDataSO() => _itemDataSO;
    public int GetCurrentStackSize() => _currentStackSize;
}
