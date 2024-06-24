using UnityEngine;
using System.Collections.Generic;

public class OpenedInventoriesManager : MonoBehaviour
{
    public static OpenedInventoriesManager Instance;

    [SerializeField] private InventoryDisplay _playerHotbarDisplay;
    [SerializeField] private InventoryDisplay _playerInventoryDisplay;
    [SerializeField] private InventoryDisplay _customContainerInventoryDisplay;

    private List<InventoryDisplay> _inventoriesDisplayList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"{this.name} instance already exists!");
            return;
        }

        Instance = this;

        _inventoriesDisplayList = new List<InventoryDisplay>();

        _inventoriesDisplayList.Add(_playerHotbarDisplay);
        _inventoriesDisplayList.Add(_playerInventoryDisplay);
    }

    public List<Inventory> GetOpenedInventories()
    {
        List<Inventory> openedInventories = new List<Inventory>();

        foreach(InventoryDisplay inventoryDisplay in _inventoriesDisplayList)
        {
            if(inventoryDisplay.IsInventoryOpened())
            {
                openedInventories.Add(inventoryDisplay.GetCurrentInventory());
            }
        }

        return openedInventories;
    }
}
