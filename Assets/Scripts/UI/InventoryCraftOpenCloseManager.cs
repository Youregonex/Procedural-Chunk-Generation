using UnityEngine;

public class InventoryCraftOpenCloseManager : MonoBehaviour
{
    [SerializeField] private DynamicInventoryDisplay _playerInventoryWindow;
    [SerializeField] private CraftingWindowDisplay _playerCraftingWindow;

    private bool _windowsOpened;

    private void Start()
    {
        PlayerInventorySystem.OnInventoryDisplayRequested += PlayerInventorySystem_OnInventoryDisplayRequested;

        HideWindows();
    }

    private void OnDestroy()
    {
        PlayerInventorySystem.OnInventoryDisplayRequested -= PlayerInventorySystem_OnInventoryDisplayRequested;
    }

    public void ShowWindows(Inventory inventory)
    {
        if (_windowsOpened)
        {
            HideWindows();
            return;
        }

        _windowsOpened = true;

        _playerInventoryWindow.ShowInventory(inventory);
        _playerCraftingWindow.DisplayCraftingWindow();
    }

    public void HideWindows()
    {
        _windowsOpened = false;

        _playerInventoryWindow.HideInventory();
        _playerCraftingWindow.HideCraftingWindow();
    }

    private void PlayerInventorySystem_OnInventoryDisplayRequested(object sender, PlayerInventorySystem.OnInventoryDisplayRequestedEventArgs e)
    {
        ShowWindows(e.inventory);
    }
}
