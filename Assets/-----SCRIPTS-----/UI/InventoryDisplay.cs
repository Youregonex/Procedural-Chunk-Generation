using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] protected RectTransform _inventorySlotUIPrefab;
    [SerializeField] protected RectTransform _inventorySlotContainer;
    [SerializeField] protected MouseItemSlot _mouseItemSlot;
    [SerializeField] protected ItemDescriptionWindow _itemDescriptionWindow;

    protected Dictionary<InventorySlot, InventorySlotUI> _inventorySlotsDictionary = new Dictionary<InventorySlot, InventorySlotUI>();
    protected List<InventorySlotUI> _inventorySlotsUIList = new List<InventorySlotUI>();
    protected bool _isOpened = false;
    protected Inventory _currentInventory;

    public bool IsOpened => _isOpened;
    public Inventory CurrentInventory => _currentInventory;


    public void InventorySlotUIClicked(InventorySlotUI clickedUISlot)
    {
        _itemDescriptionWindow.HideItemDescription();

        bool isCtrlKeyPressed = Keyboard.current.leftCtrlKey.isPressed;
        bool isShiftKeyPressed = Keyboard.current.leftShiftKey.isPressed;

        // UI slot NOT EMPTY && Mouse slot EMPTY
        if (!clickedUISlot.InventorySlotUIEmpty() && _mouseItemSlot.MouseSlotEmpty())
        {
            if (isShiftKeyPressed)
            {
                List<Inventory> openedInventoriesList = OpenedInventoriesManager.Instance.GetOpenedInventories();
                int currentAmount = clickedUISlot.AssignedInventorySlot.CurrentStackSize;

                foreach (Inventory inventory in openedInventoriesList)
                {
                    if (inventory == _currentInventory)
                        continue;

                    currentAmount = inventory.AddItemToInventory(clickedUISlot.AssignedInventorySlot.ItemDataSO,
                                                                 currentAmount);

                    if (currentAmount == clickedUISlot.AssignedInventorySlot.ItemDataSO.MaxStackSize)
                        continue;

                    if (currentAmount == 0)
                    {
                        clickedUISlot.AssignedInventorySlot.ClearSlot();
                        return;
                    }

                    clickedUISlot.AssignedInventorySlot.RemoveFromStackSize(clickedUISlot.AssignedInventorySlot.CurrentStackSize - currentAmount);
                }

                return;
            }

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
            if (isShiftKeyPressed)
            {
                List<Inventory> openedInventoriesList = OpenedInventoriesManager.Instance.GetOpenedInventories();
                int currentAmount = clickedUISlot.AssignedInventorySlot.CurrentStackSize;

                foreach (Inventory inventory in openedInventoriesList)
                {
                    if (inventory == _currentInventory)
                        continue;

                    currentAmount = inventory.AddItemToInventory(clickedUISlot.AssignedInventorySlot.ItemDataSO,
                                                                 currentAmount);

                    if (currentAmount == clickedUISlot.AssignedInventorySlot.ItemDataSO.MaxStackSize)
                        continue;

                    if (currentAmount == 0)
                    {
                        clickedUISlot.AssignedInventorySlot.ClearSlot();
                        return;
                    }

                    clickedUISlot.AssignedInventorySlot.RemoveFromStackSize(clickedUISlot.AssignedInventorySlot.CurrentStackSize - currentAmount);
                }

                return;
            }

            // UI slot item == Mouse slot item
            if (clickedUISlot.AssignedInventorySlot.ItemDataSO == _mouseItemSlot.ItemdDataSO)
            {
                if(isCtrlKeyPressed &&
                   clickedUISlot.AssignedInventorySlot.CurrentStackSize != clickedUISlot.AssignedInventorySlot.ItemDataSO.MaxStackSize)
                {
                    clickedUISlot.AssignedInventorySlot.AddToStackSize(1);
                    _mouseItemSlot.SetMouseSlot(_mouseItemSlot.ItemdDataSO, _mouseItemSlot.ItemQuantity - 1);

                    if (_mouseItemSlot.ItemQuantity <= 0)
                        _mouseItemSlot.ClearSlot();

                    return;
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

    protected InventorySlotUI CreateInventorySlotUI()
    {
        Transform slotTransform = Instantiate(_inventorySlotUIPrefab, _inventorySlotContainer);

        InventorySlotUI inventorySlotUI = slotTransform.GetComponent<InventorySlotUI>();

        _inventorySlotsUIList.Add(inventorySlotUI);

        inventorySlotUI.SetParentDisplay(this);

        inventorySlotUI.OnUISlotClicked += InventorySlotUI_OnUISlotClicked;
        inventorySlotUI.OnPointerEnterUISlot += InventorySlotUI_OnPointerEnterUISlot;
        inventorySlotUI.OnPointerExitUISlot += InventorySlotUI_OnPointerExitUISlot;

        return inventorySlotUI;
    }

    protected void InventorySlotUI_OnPointerExitUISlot(object sender, System.EventArgs e)
    {
        _itemDescriptionWindow.HideItemDescription();
    }

    protected void InventorySlotUI_OnPointerEnterUISlot(object sender, System.EventArgs e)
    {
        InventorySlotUI inventorySlotUI = sender as InventorySlotUI;

        _itemDescriptionWindow.DisplayItemDescription(inventorySlotUI);
    }

    protected void InventorySlotUI_OnUISlotClicked(object sender, System.EventArgs e)
    {
        InventorySlotUI clickedSlot = sender as InventorySlotUI;
        InventorySlotUIClicked(clickedSlot);
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
