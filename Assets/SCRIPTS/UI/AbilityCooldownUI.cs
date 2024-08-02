using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldownUI : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _abilityIcon;
    [SerializeField] private Image _fillBar;

    public void SetupElement(Sprite abilityIcon)
    {
        _abilityIcon.sprite = abilityIcon;
    }

    public void HideElement()
    {
        gameObject.SetActive(false);
    }

    public void ShowElement()
    {
        gameObject.SetActive(true);
    }

    public void UpdateFillAmount(float fill)
    {
        _fillBar.fillAmount = fill;
    }
}
