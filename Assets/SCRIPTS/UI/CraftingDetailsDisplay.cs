using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CraftingDetailsDisplay : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _craftingItemIcon;
    [SerializeField] private TextMeshProUGUI _craftingItemName;
    [SerializeField] private TextMeshProUGUI _craftingItemDescription;
    [SerializeField] private TextMeshProUGUI _amountToCraftText;
    [SerializeField] private Button _craftingButton;
    [SerializeField] private Button _increaseCraftAmountButton;
    [SerializeField] private Button _decreaseCraftAmountButton;
    [SerializeField] private Image _craftProgressBar;
    [SerializeField] private Transform _craftingComponentUIPrefab;
    [SerializeField] private List<CraftingComponentUI> _craftingComponentUIList = new List<CraftingComponentUI>();

    private PlayerInventorySystem _playerInventory;
    private CraftingRecipeSO _currentRecipe;
    private int _currentCraftItemAmount = 1;
    private int _buttonAddCraftAmountShift = 5;
    [Header("Debug Fields")]
    [SerializeField] private List<CraftingComponentUI> _activeCraftingComponentUIList = new List<CraftingComponentUI>();

    private void Awake()
    {
        _craftingButton.onClick.AddListener(() =>
        {
            _playerInventory.CraftItem(_currentRecipe, _currentCraftItemAmount);

            UpdateCraftAvailability();
        });

        _increaseCraftAmountButton.onClick.AddListener(() =>
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                _currentCraftItemAmount += _buttonAddCraftAmountShift;
            else
                _currentCraftItemAmount++;

            _amountToCraftText.text = _currentCraftItemAmount.ToString();

            UpdateCraftAvailability();
            UpdateCraftingComponentUIAmountText();
        });

        _decreaseCraftAmountButton.onClick.AddListener(() =>
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                _currentCraftItemAmount -= _buttonAddCraftAmountShift;
            else
                _currentCraftItemAmount--;

            if (_currentCraftItemAmount < 1)
                _currentCraftItemAmount = 1;

            _amountToCraftText.text = _currentCraftItemAmount.ToString();

            UpdateCraftAvailability();
            UpdateCraftingComponentUIAmountText();
        });
    }

    public void Initialize(PlayerInventorySystem playerInventorySystem)
    {
        _playerInventory = playerInventorySystem;
    }

    public void ShowCraftingDetailsWindow()
    {
        SetEmptyValues();
    }

    public void HideCraftingDisplayWindow()
    {
        HideAllCraftingComponentUI();

        _currentRecipe = null;
    }

    public void SetCraftingDetailsData(CraftingRecipeSO craftingRecipeSO, PlayerInventorySystem playerInventory)
    {
        _currentRecipe = craftingRecipeSO;

        RefreshCraftingDetailsWindow();
    }

    public void UpdateCraftAvailability()
    {
        if (_currentRecipe == null)
            return;

        bool craftAvailable = true;

        for (int i = 0; i < _activeCraftingComponentUIList.Count; i++)
        {
            CraftingComponentStruct craftingComponent = _activeCraftingComponentUIList[i].CurrentCraftingComponent;
            bool craftingComponentAvailable = _playerInventory.CraftingComponentAvailable(craftingComponent.componentItemDataSO,
                                                                                          craftingComponent.amountForCraft * _currentCraftItemAmount);

            _activeCraftingComponentUIList[i].SetComponentAvailability(craftingComponentAvailable);

            if (!craftingComponentAvailable)
                craftAvailable = false;
            
        }

        _craftingButton.interactable = craftAvailable;
    }

    private void RefreshCraftingDetailsWindow()
    {
        SetRecipeValues();
        HideAllCraftingComponentUI();

        for (int i = 0; i < _currentRecipe.RecipeComponentsList.Count; i++)
        {
            CraftingComponentUI currentCraftingComponentUI = _craftingComponentUIList[i];
            currentCraftingComponentUI.gameObject.SetActive(true);
            _activeCraftingComponentUIList.Add(currentCraftingComponentUI);

            currentCraftingComponentUI.SetCraftingComponentData(_currentRecipe.RecipeComponentsList[i], _currentCraftItemAmount);
        }

        UpdateCraftAvailability();
    }

    private void SetEmptyValues()
    {
        _craftProgressBar.fillAmount = 0f;
        _currentCraftItemAmount = 1;
        _amountToCraftText.text = _currentCraftItemAmount.ToString();

        _craftingItemName.text = "";
        _craftingItemDescription.text = "";

        _craftingButton.interactable = false;
        _increaseCraftAmountButton.interactable = false;
        _decreaseCraftAmountButton.interactable = false;
        _craftingItemIcon.color = Color.clear;
    }

    private void SetRecipeValues()
    {
        _craftingItemName.text = _currentRecipe.RecipeResult.Name;
        _craftingItemDescription.text = _currentRecipe.RecipeResult.Description;

        _craftingItemIcon.color = Color.white;
        _craftingItemIcon.sprite = _currentRecipe.RecipeResult.Icon;

        _increaseCraftAmountButton.interactable = true;
        _decreaseCraftAmountButton.interactable = true;
    }


    private void UpdateCraftingComponentUIAmountText()
    {
        foreach(CraftingComponentUI craftingComponentUI in _activeCraftingComponentUIList)
        {
            craftingComponentUI.UpdateCraftingComponentAmountText(_currentCraftItemAmount);
        }
    }

    private void HideAllCraftingComponentUI()
    {
        foreach (CraftingComponentUI craftingComponentUI in _activeCraftingComponentUIList)
        {
            craftingComponentUI.gameObject.SetActive(false);
        }

        _activeCraftingComponentUIList.Clear();
    }
}