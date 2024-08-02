using UnityEngine;
using Cinemachine;

public class GameSceneInitializer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private HotbarDisplay _playerHotbarDisplay;
    [SerializeField] private CinemachineVirtualCamera _playerFollorwCamera;
    [SerializeField] private CraftingWindowDisplay _playerCraftingWindowDisplay;
    [SerializeField] private Transform _playerSpawn;
    [SerializeField] private MouseItemSlot _mouseItemSlot;

    [Header("Debug Fields")]
    [SerializeField] private PlayerInventorySystem _playerInventory;
    [SerializeField] private PlayerCraftingSystem _playerCraftingSystem;
    [SerializeField] private PlayerCore _playerCore;

    private void Awake()
    {
        SetupScene();
    }

    private void SetupScene()
    {
        SetupPlayer();
        InitializeHotbarDisplay();
        InitializePlayerCraftingWindowDisplay();
        InitializeMouseItemSlot();
    }

    private void SetupPlayer()
    {
        GameObject player = Instantiate(_playerPrefab, _playerSpawn.position, Quaternion.identity);
        player.name = "Player";

        _playerFollorwCamera.Follow = player.transform;

        _playerCore = player.GetComponent<PlayerCore>();
        _playerInventory = player.GetComponent<PlayerInventorySystem>();
        _playerCraftingSystem = player.GetComponent<PlayerCraftingSystem>();
    }

    private void InitializeHotbarDisplay()
    {
        _playerHotbarDisplay.InitializeHotbar(_playerInventory);
    }

    private void InitializePlayerCraftingWindowDisplay()
    {
        _playerCraftingWindowDisplay.InitializeCraftingWindowDisplay(_playerCraftingSystem, _playerInventory);
    }

    private void InitializeMouseItemSlot()
    {
        _mouseItemSlot.InitializeMouseItemSlot(_playerCore);
    }
}
