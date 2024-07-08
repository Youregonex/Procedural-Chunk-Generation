using UnityEngine;
using System.Collections.Generic;
using System;

public class HotbarDisplay : InventoryDisplay
{
    public static event Action<InventorySlot> OnHotbarSlotSelected;

    [Header("Config")]
    [SerializeField] private PlayerInventorySystem _playerInventory;
    [SerializeField] private float _inventorySlotUISize = 75f;

    [Header("Debug Fields")]
    [SerializeField] private HotbarSlotUI _currentHotbarSlotUI;

    private void Awake()
    {
        _isOpened = true;
        InitializeHotbar();
    }

    private void Start()
    {
        _currentInventory = _playerInventory.GetHotbarInventory();
        _currentInventory.Inventory_OnInventorySlotChanged += Inventory_OnInventorySlotChanged;

        foreach (InventorySlotUI inventorySlotUI in _inventorySlotsUIList)
        {
            HotbarSlotUI hotbarSlotUI = (HotbarSlotUI)inventorySlotUI;
            hotbarSlotUI.DeselectSlot();
        }

        SelectSlot(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            SelectSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            SelectSlot(8);
        if (Input.GetKeyDown(KeyCode.Alpha0))
            SelectSlot(9);
    }

    private void OnDestroy()
    {
        _currentInventory.Inventory_OnInventorySlotChanged -= Inventory_OnInventorySlotChanged;
    }

    private void Inventory_OnInventorySlotChanged(object sender, EventArgs e)
    {
        RefreshSelectedSlot();
    }

    private void RefreshSelectedSlot()
    {
        OnHotbarSlotSelected(_currentHotbarSlotUI.AssignedInventorySlot);
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

    private void SelectSlot(int slotNumber)
    {
        if (_currentHotbarSlotUI == _inventorySlotsUIList[slotNumber])
            return;

        if (_currentHotbarSlotUI != null)
            _currentHotbarSlotUI.DeselectSlot();

        HotbarSlotUI hotbarSlotUI = (HotbarSlotUI)_inventorySlotsUIList[slotNumber];

        hotbarSlotUI.SelectSlot();
        _currentHotbarSlotUI = hotbarSlotUI;

        OnHotbarSlotSelected(_currentHotbarSlotUI.AssignedInventorySlot);
    }
}
