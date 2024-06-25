using UnityEngine;
using UnityEngine.UI;

public class CraftingWindowDisplay : MonoBehaviour
{
    [SerializeField] private CraftingDetailsDisplay _craftingDetailsDisplay;
    [SerializeField] private CraftingListDisplay _craftingListDisplay;
    [SerializeField] private Image _craftingWindowBackground;

    [SerializeField] private PlayerCraftingSystem _playerCraftingSystem;
    [SerializeField] private PlayerInventorySystem _playerInventorySystem;


    private void Start()
    {
        _craftingListDisplay.OnCraftRecipeDetailsDisplayRequested += CraftListDisplay_OnCraftRecipeDetailsDisplayRequested;
        _playerInventorySystem.OnInventoryContentChanged += PlayerInventorySystem_OnInventoryContentChanged;
    }

    private void OnDestroy()
    {
        _craftingListDisplay.OnCraftRecipeDetailsDisplayRequested -= CraftListDisplay_OnCraftRecipeDetailsDisplayRequested;
        _playerInventorySystem.OnInventoryContentChanged -= PlayerInventorySystem_OnInventoryContentChanged;
    }

    private void CraftListDisplay_OnCraftRecipeDetailsDisplayRequested(object sender, CraftingListDisplay.OnCraftRecipeDetailsDisplayRequestedEventArgs e)
    {
        
    }

    public void DisplayCraftingWindow()
    {
        _craftingWindowBackground.gameObject.SetActive(true);

        _craftingListDisplay.gameObject.SetActive(true);
        _craftingListDisplay.DisplayCraftingRecipes(_playerCraftingSystem.GetAvailableCraftingRecipes(), _playerInventorySystem);

        _craftingDetailsDisplay.gameObject.SetActive(true);
        _craftingDetailsDisplay.ShowCraftingDisplayWindow();
    }

    public void HideCraftingWindow()
    {
        _craftingWindowBackground.gameObject.SetActive(false);

        _craftingListDisplay.HideCrafingRecipes();
        _craftingListDisplay.gameObject.SetActive(false);

        _craftingDetailsDisplay.HideCraftingDisplayWindow();
        _craftingDetailsDisplay.gameObject.SetActive(false);
    }

    private void PlayerInventorySystem_OnInventoryContentChanged(object sender, System.EventArgs e)
    {
        _craftingListDisplay.RefreshCraftingList(_playerInventorySystem);
    }
}
