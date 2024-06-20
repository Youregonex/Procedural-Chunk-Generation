using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlotUI : MonoBehaviour
{
    public event EventHandler OnUISlotClicked;

    [SerializeField] private Image _itemImage;
    [SerializeField] private RectTransform _quantityTextBackground;
    [SerializeField] private TextMeshProUGUI _itemQuantityText;

    private Button _button;
    private InventorySlot _assignedInventorySlot;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            OnUISlotClicked?.Invoke(this, EventArgs.Empty);
            Debug.Log("Slot Clicked");
        });
    }

    public void SetSlotUI(InventorySlot slot)
    {
        _assignedInventorySlot = slot;

        if(slot.GetSlotItemDataSO() == null)
        {
            SetClearSlot();
            return;
        }

        _itemImage.color = new Color(1, 1, 1, 1);
        _itemImage.sprite = slot.GetSlotItemDataSO().Icon;

        if(slot.GetCurrentStackSize() <= 1)
        {
            _itemQuantityText.gameObject.SetActive(false);
            _quantityTextBackground.gameObject.SetActive(false);
        }
        else
        {
            _itemQuantityText.gameObject.SetActive(true);
            _quantityTextBackground.gameObject.SetActive(true);
            _itemQuantityText.text = slot.GetCurrentStackSize().ToString();
        }
    }

    private void SetClearSlot()
    {
        _itemImage.color = new Color(0,0,0,0);
        _quantityTextBackground.gameObject.SetActive(false);
    }

    public void AssignInventorySlot(InventorySlot slot) => _assignedInventorySlot = slot;
}
