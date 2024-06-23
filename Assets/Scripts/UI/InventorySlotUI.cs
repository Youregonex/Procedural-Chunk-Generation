using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private RectTransform _quantityTextBackground;
    [SerializeField] private TextMeshProUGUI _itemQuantityText;

    [SerializeField] private InventoryDisplay _parentDisplay;

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
            Debug.Log("Slot clicked");
            _parentDisplay.InventorySlotUIClicked(this);
        });
    }

    public void SetSlotUI(InventorySlot slot)
    {
        _assignedInventorySlot = slot;

        _assignedInventorySlot.OnInventorySlotChanged += InventorySlot_OnInventorySlotChanged;

        if(slot.GetSlotItemDataSO() == null)
        {
            SetClearSlot();
            return;
        }

        _itemImage.color = Color.white;
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

    private void InventorySlot_OnInventorySlotChanged(object sender, System.EventArgs e)
    {
        SetSlotUI(sender as InventorySlot);
    }

    private void SetClearSlot()
    {
        _itemImage.color = Color.clear;
        _quantityTextBackground.gameObject.SetActive(false);
    }

    public InventorySlot GetAssignedInventorySlot() => _assignedInventorySlot;
    public void AssignInventorySlot(InventorySlot slot) => _assignedInventorySlot = slot;
    public bool InventorySlotUIEmpty() => _assignedInventorySlot.GetSlotItemDataSO() == null;
    public void SetParentDisplay(InventoryDisplay inventoryDisplay) => _parentDisplay = inventoryDisplay;
}
