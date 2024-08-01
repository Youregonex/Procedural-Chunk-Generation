using UnityEngine;
using UnityEngine.UI;

public class CraftingWindowDisplay : MonoBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] private PlayerCraftingSystem _playerCraftingSystem;
    [SerializeField] private PlayerInventorySystem _playerInventorySystem;
    [SerializeField] private CraftingDetailsDisplay _craftingDetailsDisplay;
    [SerializeField] private CraftingListDisplay _craftingListDisplay;
    [SerializeField] private Image _craftingWindowBackground;


    private void OnDestroy()
    {
        _craftingListDisplay.OnCraftRecipeDetailsDisplayRequested -= CraftListDisplay_OnCraftRecipeDetailsDisplayRequested;
        _playerInventorySystem.OnInventoryContentChanged -= PlayerInventorySystem_OnInventoryContentChanged;
    }

    public void InitializeCraftingWindowDisplay(PlayerCraftingSystem playerCraftingSystem, PlayerInventorySystem playerInventorySystem)
    {
        _playerCraftingSystem = playerCraftingSystem;
        _playerInventorySystem = playerInventorySystem;

        _craftingListDisplay.OnCraftRecipeDetailsDisplayRequested += CraftListDisplay_OnCraftRecipeDetailsDisplayRequested;
        _playerInventorySystem.OnInventoryContentChanged += PlayerInventorySystem_OnInventoryContentChanged;
    }

    public void DisplayCraftingWindow()
    {
        _craftingWindowBackground.gameObject.SetActive(true);

        _craftingListDisplay.gameObject.SetActive(true);
        _craftingListDisplay.DisplayCraftingRecipes(_playerCraftingSystem.GetAvailableCraftingRecipes(), _playerInventorySystem);

        _craftingDetailsDisplay.gameObject.SetActive(true);
        _craftingDetailsDisplay.DisplayCraftingDetailsWindow();
    }

    public void HideCraftingWindow()
    {
        _craftingWindowBackground.gameObject.SetActive(false);

        _craftingListDisplay.HideCrafingRecipes();
        _craftingListDisplay.gameObject.SetActive(false);

        _craftingDetailsDisplay.HideCraftingDisplayWindow();
        _craftingDetailsDisplay.gameObject.SetActive(false);
    }

    private void CraftListDisplay_OnCraftRecipeDetailsDisplayRequested(object sender, CraftingListDisplay.OnCraftRecipeDetailsDisplayRequestedEventArgs e)
    {
        _craftingDetailsDisplay.SetCraftingDetailsData(e.currentCraftingRecipeSO, _playerInventorySystem);
    }

    private void PlayerInventorySystem_OnInventoryContentChanged(object sender, System.EventArgs e)
    {
        _craftingListDisplay.RefreshCraftingList(_playerInventorySystem);
        _craftingDetailsDisplay.UpdateCraftAvailability();
    }
}
