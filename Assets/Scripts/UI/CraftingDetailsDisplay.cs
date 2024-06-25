using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

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
    [SerializeField] private List<CraftingComponentUI> _craftingComponentUIList;

    private CraftingRecipeSO _currentRecipe;

    private void Awake()
    {
        _craftingButton.onClick.AddListener(() =>
        {

        });

        _addCraftAmountButton.onClick.AddListener(() =>
        {

        });

        _decreaseCraftAmountButton.onClick.AddListener(() =>
        {

        });
    }

    public void SetCraftingDetailsData(CraftingRecipeSO crafringRecipeSO)
    {
        _currentRecipe = crafringRecipeSO;

        UpdateCraftingDetails();
    }

    public void ShowCraftingDisplayWindow()
    {
        _craftProgressBar.fillAmount = 0f;

        if (_currentRecipe == null)
        {
            _craftingItemName.text = "";
            _craftingItemDescription.text = "";

            _craftingButton.interactable = false;
            _addCraftAmountButton.interactable = false;
            _decreaseCraftAmountButton.interactable = false;
        }
    }

    public void UpdateCraftingDetails()
    {

    }


    public void HideCraftingDisplayWindow()
    {
        foreach(CraftingComponentUI craftingComponentUI in _craftingComponentUIList)
        {
            craftingComponentUI.gameObject.SetActive(false);
        }
    }

}
