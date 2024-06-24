using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        bool isCtrlKeyPressed = Keyboard.current.leftCtrlKey.isPressed;
        bool isShiftKeyPressed = Keyboard.current.leftShiftKey.isPressed;

        // UI slot NOT EMPTY && Mouse slot EMPTY
        if (!clickedUISlot.InventorySlotUIEmpty() && _mouseItemSlot.MouseSlotEmpty())
        {
            if (isCtrlKeyPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out int splitStack))
            {
                _mouseItemSlot.SetMouseSlot(clickedUISlot.AssignedInventorySlot.ItemDataSO, splitStack);

                return;
            }
            else
            {
                _mouseItemSlot.SetMouseSlot(clickedUISlot.AssignedInventorySlot);

                clickedUISlot.AssignedInventorySlot.ClearSlot();

                return;
            }
        }

        // UI slot EMPTY && Mouse slot NOT EMPTY
        if (clickedUISlot.InventorySlotUIEmpty() && !_mouseItemSlot.MouseSlotEmpty())
        {
            if (isCtrlKeyPressed)
            {
                clickedUISlot.AssignedInventorySlot.SetSlotData(_mouseItemSlot.ItemdDataSO, 1);

                _mouseItemSlot.SetMouseSlot(_mouseItemSlot.ItemdDataSO, _mouseItemSlot.ItemQuantity - 1);

                if (_mouseItemSlot.ItemQuantity <= 0)
                    _mouseItemSlot.ClearSlot();

                return;
            }
            else
            {
                clickedUISlot.AssignedInventorySlot.SetSlotData(_mouseItemSlot.ItemdDataSO, _mouseItemSlot.ItemQuantity);

                _mouseItemSlot.ClearSlot();

                return;
            }
        }

        // UI slot NOT EMPTY && Mouse slot NOT EMPTY
        if (!clickedUISlot.InventorySlotUIEmpty() && !_mouseItemSlot.MouseSlotEmpty())
        {
            // UI slot item == Mouse slot item
            if (clickedUISlot.AssignedInventorySlot.ItemDataSO == _mouseItemSlot.ItemdDataSO)
            {
                if(isCtrlKeyPressed &&
                   _mouseItemSlot.ItemQuantity > 1 &&
                   clickedUISlot.AssignedInventorySlot.CurrentStackSize != clickedUISlot.AssignedInventorySlot.ItemDataSO.MaxStackSize)
                {


                }

                // One of slots is full
                if (clickedUISlot.AssignedInventorySlot.SlotIsFull() || _mouseItemSlot.SlotIsFull())
                {
                    SwapItems(clickedUISlot.AssignedInventorySlot);

                    return;
                }

                // UI slot can fit whole Mouse slot quantity
                if (clickedUISlot.AssignedInventorySlot.StackCanFit(_mouseItemSlot.ItemQuantity, out int stackCantFit))
                {
                    clickedUISlot.AssignedInventorySlot.AddToStackSize(_mouseItemSlot.ItemQuantity);

                    _mouseItemSlot.ClearSlot();

                    return;
                }
                // UI slot can't fit whole Mouse slot quantity
                else
                {
                    clickedUISlot.AssignedInventorySlot.SetMaxStackSize();

                    _mouseItemSlot.SetMouseSlot(_mouseItemSlot.ItemdDataSO, stackCantFit);

                    return;
                }
            }

            // UI slot item != Mouse slot item
            if (clickedUISlot.AssignedInventorySlot.ItemDataSO != _mouseItemSlot.ItemdDataSO)
            {
                SwapItems(clickedUISlot.AssignedInventorySlot);

                return;
            }
        }
    }

    private void SwapItems(InventorySlot slot)
    {
        ItemDataSO mouseItemDataSO = _mouseItemSlot.ItemdDataSO;
        int mouseItemQuantity = _mouseItemSlot.ItemQuantity;

        _mouseItemSlot.SetMouseSlot(slot.ItemDataSO, slot.CurrentStackSize);

        slot.SetSlotData(mouseItemDataSO, mouseItemQuantity);
    }

    protected void RefreshInventoryDisplay()
    {
        foreach (InventorySlotUI slotUI in _inventorySlotsUIList)
        {
            slotUI.RefreshSlotUI();
        }
    }
}
