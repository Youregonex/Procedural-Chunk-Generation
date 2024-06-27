using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CraftingDetailsDisplay : MonoBehaviour
{
    [SerializeField] private Image _craftingItemIcon;
    [SerializeField] private TextMeshProUGUI _craftingItemName;
    [SerializeField] private TextMeshProUGUI _craftingItemDescription;

    [SerializeField] private TextMeshProUGUI _amountToCraftText;
    [SerializeField] private Button _craftingButton;
    [SerializeField] private Button _addCraftAmountButton;
    [SerializeField] private Button _decreaseCraftAmountButton;
    [SerializeField] private Image _craftProgressBar;
    [SerializeField] private Transform _craftingComponentUIPrefab;
    [SerializeField] private List<CraftingComponentUI> _craftingComponentUIList = new List<CraftingComponentUI>();

    private PlayerInventorySystem _currentPlayerInventory;
    private CraftingRecipeSO _currentRecipe;
    private int _currentCraftItemAmount = 1;

    [Header("Debug Fields")]
    [SerializeField] private List<CraftingComponentUI> _activeCraftingComponentUI = new List<CraftingComponentUI>();

    private void Awake()
    {
        _craftingButton.onClick.AddListener(() =>
        {
            _currentPlayerInventory.CraftItem(_currentRecipe, _currentCraftItemAmount);

            UpdateCraftAvailability();
        });

        _addCraftAmountButton.onClick.AddListener(() =>
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                _currentCraftItemAmount += 5;
            else
                _currentCraftItemAmount++;

            _amountToCraftText.text = _currentCraftItemAmount.ToString();

            UpdateCraftAvailability();
            UpdateCraftingComponentUIAmountText();
        });

        _decreaseCraftAmountButton.onClick.AddListener(() =>
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                _currentCraftItemAmount -= 5;
            else
                _currentCraftItemAmount--;

            if (_currentCraftItemAmount < 1)
                _currentCraftItemAmount = 1;

            _amountToCraftText.text = _currentCraftItemAmount.ToString();

            UpdateCraftAvailability();
            UpdateCraftingComponentUIAmountText();
        });
    }

    public void DisplayCraftingDetailsWindow()
    {
        _craftProgressBar.fillAmount = 0f;
        _currentCraftItemAmount = 1;
        _amountToCraftText.text = _currentCraftItemAmount.ToString();

        _craftingItemName.text = "";
        _craftingItemDescription.text = "";

        _craftingButton.interactable = false;
        _addCraftAmountButton.interactable = false;
        _decreaseCraftAmountButton.interactable = false;
        _craftingItemIcon.color = Color.clear;
    }

    public void SetCraftingDetailsData(CraftingRecipeSO crafringRecipeSO, PlayerInventorySystem playerInventory)
    {
        _currentRecipe = crafringRecipeSO;
        _currentPlayerInventory = playerInventory;

        RefreshCraftingDetailsWindow();
    }

    public void RefreshCraftingDetailsWindow()
    {
        _craftingItemName.text = _currentRecipe.RecipeResult.Name;
        _craftingItemDescription.text = _currentRecipe.RecipeResult.Description;

        _craftingItemIcon.color = Color.white;
        _craftingItemIcon.sprite = _currentRecipe.RecipeResult.Icon;

        _addCraftAmountButton.interactable = true;
        _decreaseCraftAmountButton.interactable = true;

        _currentCraftItemAmount = 1;
        _amountToCraftText.text = _currentCraftItemAmount.ToString();

        HideAllCraftingComponentUI();

        for (int i = 0; i < _currentRecipe.RecipeComponentsList.Count; i++)
        {
            CraftingComponentUI currentCraftingComponentUI = _craftingComponentUIList[i];
            currentCraftingComponentUI.gameObject.SetActive(true);
            _activeCraftingComponentUI.Add(currentCraftingComponentUI);

            currentCraftingComponentUI.SetCraftingComponentData(_currentRecipe.RecipeComponentsList[i], _currentCraftItemAmount);
        }

        UpdateCraftAvailability();
    }

    public void UpdateCraftAvailability()
    {
        if (_currentRecipe == null)
            return;

        bool craftAvailable = true;

        for (int i = 0; i < _activeCraftingComponentUI.Count; i++)
        {
            CraftingComponentStruct craftingComponent = _activeCraftingComponentUI[i].CurrentCraftingComponent;
            bool craftingComponentAvailable = _currentPlayerInventory.CraftingComponentAvailable(craftingComponent.componentItemDataSO,
                                                                                                 craftingComponent.amountForCraft * _currentCraftItemAmount);

            _activeCraftingComponentUI[i].SetComponentAvailability(craftingComponentAvailable);

            if (!craftingComponentAvailable)
                craftAvailable = false;
        }

        _craftingButton.interactable = craftAvailable;
    }

    public void HideCraftingDisplayWindow()
    {
        HideAllCraftingComponentUI();

        _currentRecipe = null;
        _currentPlayerInventory = null;

        _activeCraftingComponentUI.Clear();
    }

    private void UpdateCraftingComponentUIAmountText()
    {
        foreach(CraftingComponentUI craftingComponentUI in _activeCraftingComponentUI)
        {
            craftingComponentUI.UpdateCraftingComponentAmountText(_currentCraftItemAmount);
        }
    }

    private void HideAllCraftingComponentUI()
    {
        foreach (CraftingComponentUI craftingComponentUI in _activeCraftingComponentUI)
        {
            craftingComponentUI.gameObject.SetActive(false);
        }
    }
}
