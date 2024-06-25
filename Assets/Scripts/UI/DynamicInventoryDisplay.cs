using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected RectTransform _inventoryBackground;

    protected void Start()
    {
        HideInventory();
    }

    public void ShowInventory(Inventory inventoryToDisplay)
    {
        if (_isOpened)
        {
            HideInventory();
            return;
        }

        _currentInventory = inventoryToDisplay;

        _currentInventory.OnInventorySlotChanged += Inventory_OnInventorySlotChanged;

        _inventoryBackground.gameObject.SetActive(true);

        _isOpened = true;

        for (int i = 0; i < inventoryToDisplay.InventorySize; i++)
        {
            InventorySlotUI slotUI = CreateInventorySlotUI();

            InventorySlot slot = inventoryToDisplay.InventoryContentList[i];

            slotUI.AssignInventorySlot(slot);

            slotUI.RefreshSlotUI();
        }
    }

    public void HideInventory()
    {
        ClearAllSlots();
        _isOpened = false;

        if(_currentInventory != null)
        {
            _currentInventory.OnInventorySlotChanged -= Inventory_OnInventorySlotChanged;
            _currentInventory = null;
        }

        _inventoryBackground.gameObject.SetActive(false);
    }

    private void Inventory_OnInventorySlotChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryDisplay();
    }

    private void ClearAllSlots()
    {
        foreach (InventorySlotUI slotUI in _inventorySlotsUIList)
        {
            slotUI.OnUISlotClicked -= InventorySlotUI_OnUISlotClicked;
            slotUI.OnPointerEnterUISlot -= InventorySlotUI_OnPointerEnterUISlot;
            slotUI.OnPointerExitUISlot -= InventorySlotUI_OnPointerExitUISlot;

            Destroy(slotUI.gameObject);
        }

        _inventorySlotsUIList.Clear();
    }
}
