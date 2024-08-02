using Youregone.Utilities;
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
        if ((_currentWeapon == null && _currentTool == null) || !CanAttack || Utility.PointerOverUIObject())
            return;

        if(_currentWeapon != null)
            _currentWeapon.Attack();

        if (_currentTool != null)
            _currentTool.Attack();
    }
}
