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

    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _itemQuantity;

    private void Start()
    {
        ClearSlot();
    }

    private void Update()
    {
        if(_itemDataSO == null)
            return;

        transform.position = Mouse.current.position.ReadValue() + _mouseItemOffset;
    }

    public void SetMouseSlot(InventorySlot slot)
    {

        _itemDataSO = slot.GetSlotItemDataSO();
        _itemQuantity = slot.GetCurrentStackSize();

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

    public ItemDataSO GetItemdDataSO() => _itemDataSO;
    public int GetItemQuantity() => _itemQuantity;
    public bool MouseSlotEmpty() => _itemDataSO == null;
}
