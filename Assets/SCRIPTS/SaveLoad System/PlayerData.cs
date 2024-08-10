using UnityEngine;

public class PlayerData : MonoBehaviour, IDataPersistance
{
    [Header("Debug Fields")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private PlayerInventorySystem _playerInventorySystem;
    [SerializeField] private AgentStatHealthSystem _playerHealthSystem;


    public void Initialize(PlayerCore playerCore)
    {
        _playerCore = playerCore;
        _playerInventorySystem = GetComponent<PlayerInventorySystem>();
        _playerTransform = _playerCore.SelfTransform;
        _playerHealthSystem = _playerCore.GetAgentComponent<AgentStatHealthSystem>();
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