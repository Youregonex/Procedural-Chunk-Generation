using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MouseItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemQuantityText;
    [SerializeField] private RectTransform _quantityTextBackground;
    [SerializeField] private Vector2 _mouseItemOffset;
    [SerializeField] private PlayerMovement _playerMovement;

    [Header("Debug Fields")]
    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _itemQuantity;


    public int ItemQuantity => _itemQuantity;
    public ItemDataSO ItemdDataSO => _itemDataSO;

    private void Start()
    {
        ClearSlot();
    }

    private void Update()
    {
        if(_itemDataSO == null)
            return;

        transform.position = Mouse.current.position.ReadValue() + _mouseItemOffset;

        if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject())
        {
            ItemFactory itemFactory = new ItemFactory();

            Item item = itemFactory.CreateItem(_itemDataSO, _itemQuantity);

            item.transform.position = _playerMovement.transform.position + new Vector3(_playerMovement.GetCurrentDirection().x, _playerMovement.GetCurrentDirection().y, 0);

            ClearSlot();
        }
    }

    public void SetMouseSlot(InventorySlot slot)
    {
        SetMouseSlot(slot.ItemDataSO, slot.CurrentStackSize);
    }

    public void SetMouseSlot(ItemDataSO itemDataSO, int itemQuantity)
    {
        _itemDataSO = itemDataSO;
        _itemQuantity = itemQuantity;

        if (_itemDataSO == null)
        {
            ClearSlot();
            return;
        }

        _itemImage.color = Color.white;
        _itemImage.sprite = _itemDataSO.Icon;

        if (_itemQuantity <= 1)
        {
            _itemQuantityText.gameObject.SetActive(false);
            _quantityTextBackground.gameObject.SetActive(false);
        }
        else
        {
            _itemQuantityText.gameObject.SetActive(true);
            _quantityTextBackground.gameObject.SetActive(true);
            _itemQuantityText.text = _itemQuantity.ToString();
        }
    }

    public void ClearSlot()
    {
        _itemImage.color = Color.clear;
        _itemDataSO = null;
        _itemQuantity = -1;
        _quantityTextBackground.gameObject.SetActive(false);
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public bool SlotIsFull() => _itemQuantity == _itemDataSO.MaxStackSize;
    public bool MouseSlotEmpty() => _itemDataSO == null;
}
