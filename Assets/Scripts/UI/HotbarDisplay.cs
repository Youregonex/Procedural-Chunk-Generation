using UnityEngine;
using System.Collections.Generic;

public class HotbarDisplay : InventoryDisplay
{
    [SerializeField] private PlayerInventorySystem _playerInventory;
    [SerializeField] private float _inventorySlotUISize = 75f;

    protected override void Awake()
    {
        base.Awake();

        _isOpened = true;
        InitializeHotbar();
    }

    private void Start()
    {
        _playerInventory.OnHotbarInventorySlotChanged += PlayerInventory_OnHotbarInventorySlotChanged;

        _currentInventory = _playerInventory.GetHotbarInventory();
    }

    private void PlayerInventory_OnHotbarInventorySlotChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryDisplay();
    }

    private void InitializeHotbar()
    {
        List<InventorySlot> hotbarInventoryContent = _playerInventory.GetHotbarInventory().InventoryContentList;

        for (int i = 0; i < _playerInventory.GetHotbarSize(); i++)
        {
            InventorySlotUI slotUI = CreateInventorySlotUI();

            slotUI.AssignInventorySlot(hotbarInventoryContent[i]);

            slotUI.RefreshSlotUI();

            slotUI.GetComponent<RectTransform>().sizeDelta = new Vector2(_inventorySlotUISize, _inventorySlotUISize);
        }
    }
}
