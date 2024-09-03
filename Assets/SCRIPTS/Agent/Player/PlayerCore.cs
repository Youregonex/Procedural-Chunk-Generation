using UnityEngine;

public class PlayerCore : AgentCoreBase
{
    [Header("Config")]
    [SerializeField] private PlayerItemSelection _playerItemSelection;
    [SerializeField] private PlayerBuildingSystem _playerBuildingSystem;
    [SerializeField] private PlayerObjectInteraction _playerInteraction;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private PlayerInventorySystem _playerInventorySystem;
    [SerializeField] private PlayerCraftingSystem _playerCraftingSystem;
    [SerializeField] private PlayerChunkInteraction _playerChunkInteraction;


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(IsOwner)
            GameSceneInitializer.Instance.StartSceneSetup(this);
    }

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
        _agentComponents.Add(_agentAbilitySystem);
        _agentComponents.Add(_playerItemSelection);
        _agentComponents.Add(_playerBuildingSystem);
        _agentComponents.Add(_playerInteraction);
        _agentComponents.Add(_playerData);
        _agentComponents.Add(_playerInventorySystem);
        _agentComponents.Add(_playerCraftingSystem);
        _agentComponents.Add(_playerChunkInteraction);

        InitializeDisableOnDeathList();
    }
}
