using UnityEngine;
using System;

[Serializable]
[RequireComponent(typeof(PlayerInput))]
public class PlayerInventorySystem : MonoBehaviour
{
    private const int HOTBAR_SIZE = 10;

    public static event EventHandler<OnInventoryDisplayRequestedEventArgs> OnInventoryDisplayRequested;
    public class OnInventoryDisplayRequestedEventArgs : EventArgs
    {
        public Inventory inventory;
    }

    public event EventHandler OnInventoryContentChanged;

    [SerializeField] private int _mainInventorySize;
    [SerializeField] private Inventory _hotbar;
    [SerializeField] private Inventory _mainInventory;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _hotbar = new Inventory();
        _mainInventory = new Inventory();

        _hotbar.InitializeInventory(HOTBAR_SIZE);
        _mainInventory.InitializeInventory(_mainInventorySize);
    }

    private void Start()
    {
        _playerInput.OnInventoryKeyPressed += PlayerInput_OnInventoryKeyPressed;
        _hotbar.Inventory_OnInventorySlotChanged += Inventory_OnInventorySlotChanged;
        _mainInventory.Inventory_OnInventorySlotChanged += Inventory_OnInventorySlotChanged;
    }

    private void OnDestroy()
    {
        _playerInput.OnInventoryKeyPressed -= PlayerInput_OnInventoryKeyPressed;
        _hotbar.Inventory_OnInventorySlotChanged -= Inventory_OnInventorySlotChanged;
        _mainInventory.Inventory_OnInventorySlotChanged -= Inventory_OnInventorySlotChanged;
    }

    public int AddItemToInventory(Item item)
    {
        int amountDidntFit = _hotbar.AddItemToInventory(item.ItemDataSO, item.ItemQuantity);

        if (amountDidntFit == 0)
        {
            return 0;
        }

        return _mainInventory.AddItemToInventory(item.ItemDataSO, amountDidntFit);
    }

    public bool CanCraftRecipe(CraftingRecipeSO craftingRecipeSO)
    {
        bool recipeCraftPossible = true;

        Inventory combinedPlayerInventories = GetCombinedInventory();

        foreach(CraftingComponent craftingComponent in craftingRecipeSO.RecipeComponents)
        {
            if(!combinedPlayerInventories.ContainsItemWithQuantity(craftingComponent.itemDataSO, craftingComponent.amountForCraft))
            {
                recipeCraftPossible = false;
                break;
            }
        }

        return recipeCraftPossible;
    }

    public int GetHotbarSize() => HOTBAR_SIZE;
    public Inventory GetHotbarInventory() => _hotbar;

    private void Inventory_OnInventorySlotChanged(object sender, EventArgs e)
    {
        OnInventoryContentChanged?.Invoke(this, EventArgs.Empty);
    }

    private Inventory GetCombinedInventory() // Get PlayerInventory + Hotbar
    {
        int mainInventorySize = _mainInventory.InventorySize;
        int hotbarSize = _hotbar.InventorySize;

        Inventory combinedPlayerInventories = new Inventory();

        combinedPlayerInventories.InitializeInventory(mainInventorySize + hotbarSize);

        for (int i = 0; i < combinedPlayerInventories.InventorySize; i++)
        {
            if (i < mainInventorySize)
            {
                combinedPlayerInventories.InventoryContentList[i] = _mainInventory.InventoryContentList[i];
            }
            else
            {
                combinedPlayerInventories.InventoryContentList[i] = _hotbar.InventoryContentList[i - mainInventorySize];
            }
        }

        return combinedPlayerInventories;
    }

    private void UpdatePlayerInventoriesWithCombinedInventory(Inventory combinedPlayerInventories) // Updates Player Inventory + Hotbar data with combined inventory data
    {
        int mainInventorySize = _mainInventory.InventorySize;
        int hotbarSize = _hotbar.InventorySize;

        bool slotDidntChange;

        for (int i = 0; i < combinedPlayerInventories.InventorySize; i++)
        {
            if (i < mainInventorySize)
            {
                slotDidntChange = _mainInventory.InventoryContentList[i] == combinedPlayerInventories.InventoryContentList[i];

                if (slotDidntChange)
                    continue;

                _mainInventory.InventoryContentList[i].SetSlotData(combinedPlayerInventories.InventoryContentList[i].ItemDataSO,
                                                                   combinedPlayerInventories.InventoryContentList[i].CurrentStackSize);
            }
            else
            {
                slotDidntChange = _hotbar.InventoryContentList[i - mainInventorySize] == combinedPlayerInventories.InventoryContentList[i];

                if (slotDidntChange)
                    continue;

                _hotbar.InventoryContentList[i - mainInventorySize].SetSlotData(combinedPlayerInventories.InventoryContentList[i].ItemDataSO,
                                                                                combinedPlayerInventories.InventoryContentList[i].CurrentStackSize);
            }
        }
    }

    private void PlayerInput_OnInventoryKeyPressed(object sender, System.EventArgs e)
    {
        OnInventoryDisplayRequested?.Invoke(this, new OnInventoryDisplayRequestedEventArgs
        {
            inventory = _mainInventory
        });
    }
}
