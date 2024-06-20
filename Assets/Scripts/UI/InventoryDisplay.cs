using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] protected RectTransform _inventorySlotUIPrefab;
    [SerializeField] protected RectTransform _inventorySlotContainer;

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

        return inventorySlotUI;
    }
}
