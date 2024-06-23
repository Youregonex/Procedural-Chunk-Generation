using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] protected RectTransform _inventorySlotUIPrefab;
    [SerializeField] protected RectTransform _inventorySlotContainer;
    [SerializeField] protected MouseItemSlot _mouseItemSlot;

    protected Dictionary<InventorySlot, InventorySlotUI> _inventorySlotsDictionary;
    protected List<InventorySlotUI> _inventorySlotsUIList;

    protected virtual void Awake()
    {
        _inventorySlotsUIList = new List<InventorySlotUI>();
        _inventorySlotsDictionary = new Dictionary<InventorySlot, InventorySlotUI>();
    }

    protected InventorySlotUI CreateInventorySlotUI()
    {
        Transform slotTransform = Instantiate(_inventorySlotUIPrefab, _inventorySlotContainer);

        InventorySlotUI inventorySlotUI = slotTransform.GetComponent<InventorySlotUI>();

        _inventorySlotsUIList.Add(inventorySlotUI);

        inventorySlotUI.SetParentDisplay(this);

        return inventorySlotUI;
    }

    public void InventorySlotUIClicked(InventorySlotUI clickedUISlot)
    {
        // UI slot NOT EMPTY && Mouse slot EMPTY
        if(!clickedUISlot.InventorySlotUIEmpty() && _mouseItemSlot.MouseSlotEmpty())
        {
            _mouseItemSlot.SetMouseSlot(clickedUISlot.GetAssignedInventorySlot());

            clickedUISlot.GetAssignedInventorySlot().ClearSlot();

            return;
        }

        // UI slot EMPTY && Mouse slot NOT EMPTY
        if (clickedUISlot.InventorySlotUIEmpty() && !_mouseItemSlot.MouseSlotEmpty())
        {
            clickedUISlot.GetAssignedInventorySlot().SetSlotData(_mouseItemSlot.GetItemdDataSO(), _mouseItemSlot.GetItemQuantity());

            _mouseItemSlot.ClearSlot();

            return;
        }

        // UI slot NOT EMPTY && Mouse slot NOT EMPTY
        if (!clickedUISlot.InventorySlotUIEmpty() && !_mouseItemSlot.MouseSlotEmpty())
        {
            // UI slot item == Mouse slot item
            if(clickedUISlot.GetAssignedInventorySlot().GetSlotItemDataSO() == _mouseItemSlot.GetItemdDataSO())
            {
                // One of slots is full
                if(clickedUISlot.GetAssignedInventorySlot().SlotIsFull() || _mouseItemSlot.SlotIsFull())
                {
                    SwapItems(clickedUISlot.GetAssignedInventorySlot());

                    return;
                }

                // UI slot can fit whole Mouse slot quantity
                if(clickedUISlot.GetAssignedInventorySlot().StackCanFit(_mouseItemSlot.GetItemQuantity(), out int stackCantFit))
                {
                    clickedUISlot.GetAssignedInventorySlot().AddToStackSize(_mouseItemSlot.GetItemQuantity());

                    _mouseItemSlot.ClearSlot();

                    return;
                }
                // UI slot can't fit whole Mouse slot quantity
                else
                {
                    clickedUISlot.GetAssignedInventorySlot().SetMaxStackSize();

                    _mouseItemSlot.SetMouseSlot(_mouseItemSlot.GetItemdDataSO(), stackCantFit);

                    return;
                }
            }

            // UI slot item != Mouse slot item
            if (clickedUISlot.GetAssignedInventorySlot().GetSlotItemDataSO() != _mouseItemSlot.GetItemdDataSO())
            {
                SwapItems(clickedUISlot.GetAssignedInventorySlot());
            }
        }
    }

    private void SwapItems(InventorySlot slot)
    {
        ItemDataSO mouseItemDataSO = _mouseItemSlot.GetItemdDataSO();
        int mouseItemQuantity = _mouseItemSlot.GetItemQuantity();

        _mouseItemSlot.SetMouseSlot(slot.GetSlotItemDataSO(), slot.GetCurrentStackSize());

        slot.SetSlotData(mouseItemDataSO, mouseItemQuantity);
    }

    protected void RefreshInventoryDisplay()
    {
        foreach (InventorySlotUI slotUI in _inventorySlotsUIList)
        {
            slotUI.SetSlotUI(slotUI.GetAssignedInventorySlot());
        }
    }
}
