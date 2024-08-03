using UnityEngine;

public class PlayerCore : AgentCoreBase
{
    [Header("Config")]
    [SerializeField] private PlayerItemSelection _playerItemSelection;
    [SerializeField] private PlayerBuildingSystem _playerBuildingSystem;
    [SerializeField] private PlayerObjectInteraction _playerInteraction;
   

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
        _agentComponents.Add(_playerBuildingSystem);
        _agentComponents.Add(_playerInteraction);

        InitializeDisableOnDeathList();
    }
}
