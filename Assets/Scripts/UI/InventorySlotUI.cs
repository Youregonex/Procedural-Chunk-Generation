using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler OnUISlotClicked;
    public event EventHandler OnPointerEnterUISlot;
    public event EventHandler OnPointerExitUISlot;


    [SerializeField] private Image _itemImage;
    [SerializeField] private RectTransform _quantityTextBackground;
    [SerializeField] private TextMeshProUGUI _itemQuantityText;

    [SerializeField] private InventoryDisplay _parentDisplay;

    private Button _button;
    private InventorySlot _assignedInventorySlot;


    public InventorySlot AssignedInventorySlot => _assignedInventorySlot;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            Debug.Log("Slot clicked");
            OnUISlotClicked?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnDestroy()
    {
        _assignedInventorySlot.OnInventorySlotChanged -= InventorySlot_OnInventorySlotChanged;
    }

    public void AssignInventorySlot(InventorySlot slot)
    {
        _assignedInventorySlot = slot;

        _assignedInventorySlot.OnInventorySlotChanged += InventorySlot_OnInventorySlotChanged;
        RefreshSlotUI();
    }

    public bool InventorySlotUIEmpty() => _assignedInventorySlot.ItemDataSO == null;
    public void SetParentDisplay(InventoryDisplay inventoryDisplay) => _parentDisplay = inventoryDisplay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InventorySlotUIEmpty())
            OnPointerEnterUISlot?.Invoke(this, EventArgs.Empty);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitUISlot?.Invoke(this, EventArgs.Empty);
    }

    public void RefreshSlotUI()
    {
        if(_assignedInventorySlot == null)
        {
            Debug.LogError($"There is no Inventory slot assigned to this {gameObject}");
            return;
        }

        if (_assignedInventorySlot.ItemDataSO == null)
        {
            SetClearSlot();
            return;
        }

        _itemImage.color = Color.white;
        _itemImage.sprite = _assignedInventorySlot.ItemDataSO.Icon;

        if (_assignedInventorySlot.CurrentStackSize <= 1)
        {
            _itemQuantityText.gameObject.SetActive(false);
            _quantityTextBackground.gameObject.SetActive(false);
        }
        else
        {
            _itemQuantityText.gameObject.SetActive(true);
            _quantityTextBackground.gameObject.SetActive(true);
            _itemQuantityText.text = _assignedInventorySlot.CurrentStackSize.ToString();
        }
    }

    private void InventorySlot_OnInventorySlotChanged(object sender, System.EventArgs e)
    {
        RefreshSlotUI();
    }

    private void SetClearSlot()
    {
        _itemImage.color = Color.clear;
        _quantityTextBackground.gameObject.SetActive(false);
    }
}
