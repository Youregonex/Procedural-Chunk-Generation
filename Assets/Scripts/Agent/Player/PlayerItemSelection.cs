using UnityEngine;
using System;

public class PlayerItemSelection : AgentMonobehaviourComponent
{
    public event Action<ItemDataSO> OnCurrentItemChanged;

    [Header("Debug Field")]
    [SerializeField] private ItemDataSO _currentItem;


    public override void DisableComponent()
    {
        this.enabled = false;
    }

    private void Start()
    {
        HotbarDisplay.OnHotbarSlotSelected += HotbarDisplay_OnHotbarSlotSelected;    
    }

    private void HotbarDisplay_OnHotbarSlotSelected(ItemDataSO itemDataSO)
    {
        _currentItem = itemDataSO;

        OnCurrentItemChanged(itemDataSO);
    }
}
