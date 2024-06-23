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
    }

    protected void RefreshInventoryDisplay()
    {
        foreach (InventorySlotUI slotUI in _inventorySlotsUIList)
        {
            slotUI.SetSlotUI(slotUI.GetAssignedInventorySlot());
        }
    }


}
