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

    public void CraftItem(CraftingRecipeSO craftingRecipeSO, int craftItemAmount)
    {
        Inventory combinedPlayerInventories = GetCombinedInventory();

        foreach (CraftingComponentStruct craftingComponent in craftingRecipeSO.RecipeComponentsList)
        {
            combinedPlayerInventories.RemoveItem(craftingComponent.componentItemDataSO, craftingComponent.amountForCraft * craftItemAmount);
        }

        int inventoryCantFit = combinedPlayerInventories.AddItemToInventory(craftingRecipeSO.RecipeResult, craftItemAmount);

        if(inventoryCantFit != 0)
        {
            ItemFactory itemFactory = new ItemFactory();

            itemFactory.CreateItem(craftingRecipeSO.RecipeResult, craftItemAmount);
        }
    }

    public bool CanCraftRecipe(CraftingRecipeSO craftingRecipeSO)
    {
        bool recipeCraftPossible = true;

        Inventory combinedPlayerInventories = GetCombinedInventory();

        foreach(CraftingComponentStruct craftingComponent in craftingRecipeSO.RecipeComponentsList)
        {
            if(!combinedPlayerInventories.ContainsItemWithQuantity(craftingComponent.componentItemDataSO, craftingComponent.amountForCraft))
            {
                recipeCraftPossible = false;
                break;
            }
        }

        return recipeCraftPossible;
    }

    public bool CraftingComponentAvailable(ItemDataSO itemDataSO, int amount)
    {
        Inventory combinedPlayerInventories = GetCombinedInventory();

        if(combinedPlayerInventories.ContainsItemWithQuantity(itemDataSO, amount))
        {
            return true;
        }

        return false;
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
            if (i < hotbarSize)
            {
                combinedPlayerInventories.InventoryContentList[i] = _hotbar.InventoryContentList[i];
            }
            else
            {
                combinedPlayerInventories.InventoryContentList[i] = _mainInventory.InventoryContentList[i - hotbarSize];
            }
        }

        return combinedPlayerInventories;
    }

    private void PlayerInput_OnInventoryKeyPressed()
    {
        OnInventoryDisplayRequested?.Invoke(this, new OnInventoryDisplayRequestedEventArgs
        {
            inventory = _mainInventory
        });
    }
}
