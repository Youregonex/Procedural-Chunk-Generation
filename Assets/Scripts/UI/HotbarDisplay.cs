using UnityEngine;
using System.Collections.Generic;

public class HotbarDisplay : InventoryDisplay
{
    [SerializeField] private PlayerInventorySystem _playerInventory;

    protected override void Awake()
    {
        InitializeHotbar();
    }

    private void InitializeHotbar()
    {
        List<InventorySlot> hotbarInventoryContent = _playerInventory.GetHotbarInventory().GetInventoryList();

        for (int i = 0; i < _playerInventory.GetHotbarSize(); i++)
        {
            InventorySlotUI slotUI = CreateInventorySlotUI();

            slotUI.SetSlotUI(hotbarInventoryContent[i]);
        }
    }
}
