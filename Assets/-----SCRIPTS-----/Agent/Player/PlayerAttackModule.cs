using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerAttackModule : AgentAttackModule
{
    [Header("Debug Field")]
    [SerializeField] private PlayerItemSelection _playerItemSelection;

    protected override void Start()
    {
        base.Start();

        _playerItemSelection = _agentCore.GetAgentComponent<PlayerItemSelection>();

        _playerItemSelection.OnCurrentItemChanged += PlayerItemSelection_OnCurrentItemChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _playerItemSelection.OnCurrentItemChanged -= PlayerItemSelection_OnCurrentItemChanged;
    }

    private void PlayerItemSelection_OnCurrentItemChanged(ItemDataSO itemDataSO)
    {
        if (itemDataSO == null || (itemDataSO.ItemType != EItemType.Weapon && itemDataSO.ItemType != EItemType.Tool))
        {
            HideCurrentWeapon();
            HideCurrentTool();
            return;
        }

        if(itemDataSO.ItemType == EItemType.Weapon)
        {
            WeaponItemDataSO weaponItemDataSO = itemDataSO as WeaponItemDataSO;

            ChangeWeapon(weaponItemDataSO);
        }

        if (itemDataSO.ItemType == EItemType.Tool)
        {
            ToolItemDataSO toolDataItemSO = itemDataSO as ToolItemDataSO;

            ChangeTool(toolDataItemSO);
        }
    }

    protected override void Attack()
    {
        if ((_currentWeapon == null && _currentTool == null) || !CanAttack || IsPointerOverUIObject())
            return;

        if(_currentWeapon != null)
            _currentWeapon.Attack();

        if (_currentTool != null)
            _currentTool.Attack();
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
