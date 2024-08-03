using UnityEngine;
using UnityEngine.UI;
using System;

public class CraftingRecipeUI : MonoBehaviour
{
    public event EventHandler<OnCraftingRecipeUIClickedEventArgs> OnCraftingRecipeUIClicked;
    public class OnCraftingRecipeUIClickedEventArgs : EventArgs
    {
        public CraftingRecipeSO currentCraftingRecipeSO;
    }

    [SerializeField] private Image _craftAvailableBackgroundColor;
    [SerializeField] private Image _craftRecipItemIcon;
    [SerializeField] private Button _craftRecipeButton;

    [SerializeField] private Color _canCraftColor;
    [SerializeField] private Color _canNotCraftColor;

    private CraftingRecipeSO _currentCraftingRecipeSO;
    private bool _craftPossible = false;

    public CraftingRecipeSO CurrentCraftingRecipeSO => _currentCraftingRecipeSO;


    private void Awake()
    {
        _craftRecipeButton.onClick.AddListener(() =>
        {
            OnCraftingRecipeUIClicked?.Invoke(this, new OnCraftingRecipeUIClickedEventArgs
            {
                currentCraftingRecipeSO = _currentCraftingRecipeSO
            });
        });
    }

    public void ResetScale()
    {
        transform.localScale = Vector2.one;
    }

    public void SetCraftingRecipeUIData(CraftingRecipeSO craftingRecipeSO, bool craftPossible)
    {
        _currentCraftingRecipeSO = craftingRecipeSO;
        _craftRecipItemIcon.sprite = _currentCraftingRecipeSO.RecipeResult.Icon;

        UpdateCraftingRecipeUIData(craftPossible);
    }

    public void UpdateCraftingRecipeUIData(bool craftPossible)
    {
        _craftPossible = craftPossible;

        if (craftPossible)
        {
            _craftAvailableBackgroundColor.color = _canCraftColor;
        }
        else
        {
            _craftAvailableBackgroundColor.color = _canNotCraftColor;
        }
    }

    public void DestroyCraftingRecipUI()
    {
        Destroy(gameObject);
    }
}
