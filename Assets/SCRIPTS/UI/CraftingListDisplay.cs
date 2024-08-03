using UnityEngine;
using System.Collections.Generic;
using System;

public class CraftingListDisplay : MonoBehaviour
{
    public event EventHandler<OnCraftRecipeDetailsDisplayRequestedEventArgs> OnCraftRecipeDetailsDisplayRequested;
    public class OnCraftRecipeDetailsDisplayRequestedEventArgs : EventArgs
    {
        public CraftingRecipeSO currentCraftingRecipeSO;
    }

    [SerializeField] private Transform _craftRecipeUIParentTransform;
    [SerializeField] private Transform _crafingRecipeUIPrefab;

    private List<CraftingRecipeUI> _craftRecipeUIList = new List<CraftingRecipeUI>();
    private List<CraftingRecipeUI> _activeRecipeUIList = new List<CraftingRecipeUI>();


    private void OnDestroy()
    {
        foreach(CraftingRecipeUI craftingRecipeUI in _craftRecipeUIList)
        {
            craftingRecipeUI.OnCraftingRecipeUIClicked -= CraftingRecipeUI_OnCraftRecipeUIClicked;
        }

        _craftRecipeUIList.Clear();
    }

    public void DisplayCraftingRecipes(List<CraftingRecipeSO> availableCrafts, PlayerInventorySystem playerInventory)
    {
        if (_craftRecipeUIList.Count < availableCrafts.Count)
        {
            for (int i = 0; i < availableCrafts.Count; i++)
            {
                CreateAndSetupCraftRecipeUI();
            }
        }

        for (int i = 0; i < availableCrafts.Count; i++)
        {
            _craftRecipeUIList[i].gameObject.SetActive(true);
            _activeRecipeUIList.Add(_craftRecipeUIList[i]);
            _craftRecipeUIList[i].ResetScale();

            bool recipeCraftPossible = playerInventory.CanCraftRecipe(availableCrafts[i]);

            _craftRecipeUIList[i].SetCraftingRecipeUIData(availableCrafts[i], recipeCraftPossible);
        }
    }

    public void RefreshCraftingList(PlayerInventorySystem playerInventory)
    {
        foreach(CraftingRecipeUI craftingRecipeUI in _activeRecipeUIList)
        {
            bool craftPossible = playerInventory.CanCraftRecipe(craftingRecipeUI.CurrentCraftingRecipeSO);
            craftingRecipeUI.UpdateCraftingRecipeUIData(craftPossible);
        }
    }

    public void HideCrafingRecipes()
    {
        if (_activeRecipeUIList.Count == 0)
            return;

        for (int i = 0; i < _activeRecipeUIList.Count; i++)
        {
            _activeRecipeUIList[i].gameObject.SetActive(false);
            _activeRecipeUIList.Remove(_activeRecipeUIList[i]);
        }
    }

    private CraftingRecipeUI CreateAndSetupCraftRecipeUI()
    {
        Transform craftingRecipeUITransform = Instantiate(_crafingRecipeUIPrefab);
        craftingRecipeUITransform.SetParent(_craftRecipeUIParentTransform);

        CraftingRecipeUI craftingRecipeUI = craftingRecipeUITransform.GetComponent<CraftingRecipeUI>();
        craftingRecipeUI.OnCraftingRecipeUIClicked += CraftingRecipeUI_OnCraftRecipeUIClicked;
        _craftRecipeUIList.Add(craftingRecipeUI);

        return craftingRecipeUI;
    }

    private void CraftingRecipeUI_OnCraftRecipeUIClicked(object sender, CraftingRecipeUI.OnCraftingRecipeUIClickedEventArgs e)
    {
        OnCraftRecipeDetailsDisplayRequested?.Invoke(this, new OnCraftRecipeDetailsDisplayRequestedEventArgs
        {
            currentCraftingRecipeSO = e.currentCraftingRecipeSO
        });
    }
}