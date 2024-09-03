using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Youregone.Utilities;
using Unity.Netcode;

public class MouseItemSlot : NetworkBehaviour
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
    [SerializeField] private PlayerInput _playerInput;

    public int ItemQuantity => _itemQuantity;
    public ItemDataSO ItemdDataSO => _itemDataSO;


    public void Initialize(PlayerCore playerCore)
    {
        _playerCore = playerCore;
        _playerInput = _playerCore.GetAgentComponent<PlayerInput>();

        _playerInput.OnMousePrimary += PlayerInput_OnMousePrimary;
    }

    private void Start()
    {
        ClearSlot();
    }

    private void Update()
    {
        if (_itemDataSO == null)
            return;

        transform.position = Mouse.current.position.ReadValue() + _mouseItemOffset;
    }

    public override void OnDestroy()
    {
        if (_playerInput != null)
            _playerInput.OnMousePrimary -= PlayerInput_OnMousePrimary;
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

    public bool SlotIsFull() => _itemQuantity == _itemDataSO.MaxStackSize;
    public bool MouseSlotEmpty() => _itemDataSO == null;


    private void PlayerInput_OnMousePrimary(object sender, System.EventArgs e)
    {
        if (_itemDataSO == null || Utility.PointerOverUIObject())
            return;

        Vector2 itemDropTargetDirection = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        SpawnItemServerRpc(MultiplayerPrefabDatabase.Instance.GetIdWithItemDataSO(_itemDataSO), _itemQuantity, _playerCore.transform.position, itemDropTargetDirection);
        ClearSlot();
    }

    [Rpc(SendTo.Server)]
    private void SpawnItemServerRpc(int itemDataSOId, int quantity, Vector2 position, Vector2 dropDirection)
    {
        ItemDataSO itemDataSO = MultiplayerPrefabDatabase.Instance.GetItemDataSOWithId(itemDataSOId);

        Item item = WorldItemSpawner.Instance.SpawnItem(itemDataSO, quantity);
        item.NetworkObject.Spawn();

        item.transform.position = position;
        item.DropInDirection(dropDirection);
    }
}