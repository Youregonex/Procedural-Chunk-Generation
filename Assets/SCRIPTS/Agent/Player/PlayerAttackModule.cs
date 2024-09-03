using Youregone.Utilities;
using UnityEngine;
using System.Collections.Generic;

public class PlayerAttackModule : AgentAttackModule
{
    [Header("Debug Field")]
    [SerializeField] private PlayerItemSelection _playerItemSelection;

    public override void Initialize()
    {
        base.Initialize();

        _playerItemSelection = AgentCore.GetAgentComponent<PlayerItemSelection>();
        _playerItemSelection.OnCurrentItemChanged += PlayerItemSelection_OnCurrentItemChanged;

        UpdateExistingAgentHoldPoints();

        if (IsOwner)
            UpdateExistingAgentHoldPoints();
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (_playerItemSelection != null)
            _playerItemSelection.OnCurrentItemChanged -= PlayerItemSelection_OnCurrentItemChanged;
    }

    protected override void Attack()
    {
        if (_currentWeapon == null || !CanAttack || Utility.PointerOverUIObject())
            return;

        _currentWeapon.Attack();
    }

    private List<AgentCoreBase> FindAgentCores()
    {
        AgentCoreBase[] playerCores = FindObjectsOfType<AgentCoreBase>();
        List<AgentCoreBase> playerCoreList = new(playerCores);

        return playerCoreList;
    }

    private void UpdateExistingAgentHoldPoints()
    {
        List<AgentCoreBase> agentCores = FindAgentCores();

        if (agentCores.Count > 1)
        {
            for (int i = 0; i < agentCores.Count; i++)
            {
                if (agentCores[i] == AgentCore)
                    continue;

                agentCores[i].UpdateAgentItemHoldPoint();
            }
        }
    }

    private void PlayerItemSelection_OnCurrentItemChanged(ItemDataSO itemDataSO)
    {
        if (itemDataSO == null || (itemDataSO.ItemType != EItemType.Weapon && itemDataSO.ItemType != EItemType.Tool))
        {
            HideCurrentWeapon();
            return;
        }

        WeaponItemDataSO weaponItemDataSO = itemDataSO as WeaponItemDataSO;
        ChangeWeapon(weaponItemDataSO);
    }
}
