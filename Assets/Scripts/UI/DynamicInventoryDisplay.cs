using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] protected RectTransform _inventoryBackground;

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

        _inventoryBackground.gameObject.SetActive(true);

        _isOpened = true;

        for (int i = 0; i < inventoryToDisplay.GetInventorySize(); i++)
        {
            InventorySlotUI slotUI = CreateInventorySlotUI();

            InventorySlot slot = inventoryToDisplay.GetInventoryList()[i];

            slotUI.SetSlotUI(slot);
        }
    }

    private void HideInventory()
    {
        ClearAllSlots();
        _isOpened = false;

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
