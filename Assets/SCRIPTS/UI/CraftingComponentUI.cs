using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CraftingComponentUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _componentIcon;
    [SerializeField] private TextMeshProUGUI _componentAmountText;

    [SerializeField] private Image _componentAmountTextBackground;
    [SerializeField] private Image _componentBackground;

    [SerializeField] private Color _componentAvailableColor;
    [SerializeField] private Color _componentNotAvailableColor;

    private CraftingComponentStruct _currentCraftingComponent;
    public CraftingComponentStruct CurrentCraftingComponent => _currentCraftingComponent;

    public void SetCraftingComponentData(CraftingComponentStruct craftingComponent, int currentCraftCountAmount)
    {
        _currentCraftingComponent = craftingComponent;
        _componentIcon.sprite = _currentCraftingComponent.componentItemDataSO.Icon;

        UpdateCraftingComponentAmountText(currentCraftCountAmount);
    }

    public void UpdateCraftingComponentAmountText(int currentCraftCountAmount)
    {
        int amountToDisplay = _currentCraftingComponent.amountForCraft * currentCraftCountAmount;

        if (amountToDisplay <= 1)
            _componentAmountTextBackground.gameObject.SetActive(false);
        else
        {
            _componentAmountTextBackground.gameObject.SetActive(true);
            _componentAmountText.text = amountToDisplay.ToString();
        }
    }

    public void SetComponentAvailability(bool craftComponentAvailable)
    {
        if (craftComponentAvailable)
            _componentBackground.color = _componentAvailableColor;
        else
            _componentBackground.color = _componentNotAvailableColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIComponentProvider.Instance.ItemDescriptionWindow.DisplayItemDescription(_currentCraftingComponent.componentItemDataSO, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIComponentProvider.Instance.ItemDescriptionWindow.HideItemDescription();
    }
}
