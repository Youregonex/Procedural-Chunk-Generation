using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected RectTransform _inventoryBackground;

    private Inventory _currentInventory;
    private bool _isOpened = false;

    protected void Start()
    {
        PlayerInventorySystem.OnInventoryDisplayRequested += PlayerInventorySystem_OnInventoryDisplayRequested;

        HideInventory();
    }

    private void PlayerInventorySystem_OnInventoryDisplayRequested(object sender, PlayerInventorySystem.OnInventoryDisplayRequestedEventArgs e)
    {
        ShowInventory(e.inventory);
    }

    private void ShowInventory(Inventory inventoryToDisplay)
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

    private void Inventory_OnInventorySlotChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryDisplay();
    }

    private void HideInventory()
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

    private void ClearAllSlots()
    {
        foreach (InventorySlotUI slotUI in _inventorySlotsUIList)
        {
            Destroy(slotUI.gameObject);
        }

        _inventorySlotsUIList.Clear();
    }

    private void OnDisable()
    {
        PlayerInventorySystem.OnInventoryDisplayRequested -= PlayerInventorySystem_OnInventoryDisplayRequested;
    }
}
