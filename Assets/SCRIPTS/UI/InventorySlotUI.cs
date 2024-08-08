using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public event EventHandler OnUISlotClicked;

    [Header("Config")]
    [SerializeField] protected Image _itemImage;
    [SerializeField] protected RectTransform _quantityTextBackground;
    [SerializeField] protected TextMeshProUGUI _itemQuantityText;

    [Header("Debug Fields")]
    [SerializeField] protected InventoryDisplay _parentDisplay;

    protected Button _button;
    protected InventorySlot _assignedInventorySlot;


    public InventorySlot AssignedInventorySlot => _assignedInventorySlot;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(() =>
        {
            OnUISlotClicked?.Invoke(this, EventArgs.Empty);
        });
    }

    private void OnDestroy()
    {
        _assignedInventorySlot.InventorySlot_OnInventorySlotChanged -= InventorySlot_OnInventorySlotChanged;
    }

    public void AssignInventorySlot(InventorySlot slot)
    {
        _assignedInventorySlot = slot;

        _assignedInventorySlot.InventorySlot_OnInventorySlotChanged += InventorySlot_OnInventorySlotChanged;
        RefreshSlotUI();
    }

    public bool InventorySlotUIEmpty() => _assignedInventorySlot.ItemDataSO == null;
    public void SetParentDisplay(InventoryDisplay inventoryDisplay) => _parentDisplay = inventoryDisplay;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIComponentProvider.Instance.ItemDescriptionWindow.DisplayItemDescription(_assignedInventorySlot.ItemDataSO, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIComponentProvider.Instance.ItemDescriptionWindow.HideItemDescription();
    }

    public void RefreshSlotUI()
    {
        if(_assignedInventorySlot == null)
        {
            Debug.LogError($"There is no Inventory slot assigned to this {gameObject.name}");
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
