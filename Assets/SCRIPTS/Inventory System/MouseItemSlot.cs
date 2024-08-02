using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Youregone.Utilities;

public class MouseItemSlot : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemQuantityText;
    [SerializeField] private RectTransform _quantityTextBackground;
    [SerializeField] private Vector2 _mouseItemOffset;

    [Header("Debug Fields")]
    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _itemQuantity;
    [SerializeField] private PlayerCore _playerCore;

    public int ItemQuantity => _itemQuantity;
    public ItemDataSO ItemdDataSO => _itemDataSO;

    private ItemFactory _itemFactory = new ItemFactory();


    private void Start()
    {
        ClearSlot();
    }

    private void Update()
    {
        if(_itemDataSO == null)
            return;

        transform.position = Mouse.current.position.ReadValue() + _mouseItemOffset;

        if (Mouse.current.leftButton.wasPressedThisFrame && !Utility.PointerOverUIObject())
        {
            Item item = _itemFactory.CreateItem(_itemDataSO, _itemQuantity);

            item.transform.position = _playerCore.transform.position;

            Vector2 mouseMoveDirectionNormalized = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            item.MoveInDirection(mouseMoveDirectionNormalized);
            ClearSlot();
        }
    }

    public void SetMouseSlot(InventorySlot slot)
    {
        SetMouseSlot(slot.ItemDataSO, slot.CurrentStackSize);
    }

    public void InitializeMouseItemSlot(PlayerCore playerCore)
    {
        _playerCore = playerCore;
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

    public bool SlotIsFull() => _itemQuantity == _itemDataSO.MaxStackSize;
    public bool MouseSlotEmpty() => _itemDataSO == null;
}
