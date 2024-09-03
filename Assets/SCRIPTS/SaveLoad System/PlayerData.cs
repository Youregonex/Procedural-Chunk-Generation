using UnityEngine;

public class PlayerData : AgentNetworkBehaviourComponent, IDataPersistance
{
    [Header("Debug Fields")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerInventorySystem _playerInventorySystem;
    [SerializeField] private AgentStatHealthSystem _playerHealthSystem;

    public PlayerCore PlayerCore => AgentCore as PlayerCore;


    public override void Initialize()
    {
        GetAgentCore();
        _playerInventorySystem = GetComponent<PlayerInventorySystem>();
        _playerTransform = PlayerCore.SelfTransform;
        _playerHealthSystem = PlayerCore.GetAgentComponent<AgentStatHealthSystem>();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    public void LoadData(GameData gameData)
    {
        _playerTransform.position = gameData.playerPosition;

        LoadPlayerInventory(gameData);
        LoadPlayerHealth(gameData);
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.playerPosition = _playerTransform.position;
        gameData.playerHotbar = _playerInventorySystem.Hotbar;
        gameData.playerMainInventory = _playerInventorySystem.MainInventory;
        gameData.playerCurrentHealth = _playerHealthSystem.CurrentHealth;
        gameData.playerMaxHealth = _playerHealthSystem.MaxHealth;
    }

    private void LoadPlayerHealth(GameData gameData)
    {
        _playerHealthSystem.SetMaxHealth(gameData.playerMaxHealth);
        _playerHealthSystem.SetCurrentHealth(gameData.playerCurrentHealth);
    }

    private void LoadPlayerInventory(GameData gameData)
    {
        _playerInventorySystem.InitializePlayerInventoryFromSave(gameData.playerHotbar, gameData.playerMainInventory);
    }

}