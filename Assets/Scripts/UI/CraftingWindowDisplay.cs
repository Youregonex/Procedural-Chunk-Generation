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
    }

    private void OnDestroy()
    {
        _craftingListDisplay.OnCraftRecipeDetailsDisplayRequested -= CraftListDisplay_OnCraftRecipeDetailsDisplayRequested;
    }

    private void CraftListDisplay_OnCraftRecipeDetailsDisplayRequested(object sender, CraftingListDisplay.OnCraftRecipeDetailsDisplayRequestedEventArgs e)
    {
        
    }

    public void DisplayCraftingWindow()
    {
        _craftingWindowBackground.gameObject.SetActive(true);
        _craftingDetailsDisplay.gameObject.SetActive(true);
        _craftingListDisplay.DisplayCraftingRecipes(_playerCraftingSystem.GetAvailableCrafts(), _playerInventorySystem);
    }

    public void HideCraftingWindow()
    {
        _craftingWindowBackground.gameObject.SetActive(false);
        _craftingDetailsDisplay.gameObject.SetActive(false);
        _craftingListDisplay.HideCrafingRecipes();
    }
}
