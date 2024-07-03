using UnityEngine;

public class PlayerCore : AgentCoreBase
{
    [Header("Config")]
    [SerializeField] private PlayerItemSelection _playerItemSelection;

    protected override void InitializeComponentList()
    {
        _agentComponents.Add(_healthSystem);
        _agentComponents.Add(_agentAttackModule);
        _agentComponents.Add(_agentMovement);
        _agentComponents.Add(_agentAnimation);
        _agentComponents.Add(_agentVisual);
        _agentComponents.Add(_agentHitbox);
        _agentComponents.Add(_agentInput);
        _agentComponents.Add(_agentStats);
        _agentComponents.Add(_playerItemSelection);

        InitializeDisableOnDeathList();
    }
}
