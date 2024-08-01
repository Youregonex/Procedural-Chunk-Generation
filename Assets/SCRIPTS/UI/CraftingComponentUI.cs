using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingComponentUI : MonoBehaviour
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

        UpdateCraftingComponentData(currentCraftCountAmount);
    }

    public void UpdateCraftingComponentData(int currentCraftCountAmount)
    {
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
}
