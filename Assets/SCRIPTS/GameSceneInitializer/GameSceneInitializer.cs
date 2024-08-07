using UnityEngine;
using Cinemachine;

public class GameSceneInitializer : MonoBehaviour
{
    [Header("Player Config")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private CinemachineVirtualCamera _playerFollowCamera;
    [SerializeField] private Transform _playerSpawn;

    [Header("UI Config")]
    [SerializeField] private CraftingWindowDisplay _playerCraftingWindowDisplay;
    [SerializeField] private HotbarDisplay _playerHotbarDisplay;
    [SerializeField] private MouseItemSlot _mouseItemSlot;
    [SerializeField] private PlayerHealthbarUI _playerHealthbarUI;
    [SerializeField] private AbilityCooldownUIDisplay _abilityCooldownUIDisplay;
    [SerializeField] private GameOverScreen _gameOverScreen;
    [SerializeField] private PreloadScreen _preloadScreen;
    [SerializeField] private TestSaveLoad _testSaveLoad;

    [Header("Managers")]
    [SerializeField] private DataPersistanceManager _dataPersistanceManager;
    [SerializeField] private ChunkGenerator _chunkGenerator;

    [Header("Debug Fields")]
    [SerializeField] private PlayerInventorySystem _playerInventory;
    [SerializeField] private PlayerCraftingSystem _playerCraftingSystem;
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private AgentHealthSystem _playerHealthSystem;
    [SerializeField] private PlayerAbilitySystem _playerAbilitySystem;

    private void Awake()
    {
        SetupScene();
    }

    private void SetupScene()
    {
        InitializePreloadScreen();

        SetupPlayer();
        InitializHotbar();
        InitializePlayerCraftingWindowDisplay();
        InitializeMouseItemSlot();
        InitializePlayereHealthbar();
        InitializeAbilityCooldownUIDisplay();
        InitializeGameOverScreen();

        InitializePlayerAbilitySystem();
        InitializeTestSaveLoad();
    }

    private void InitializePlayerAbilitySystem() // Initialize after AbilityCooldownDisplay
    {
        _playerAbilitySystem.InitializePlayerAbilitySystem();
    }

    private void SetupPlayer()
    {
        GameObject player = Instantiate(_playerPrefab, _playerSpawn.position, Quaternion.identity);
        player.name = "Player";

        _playerFollowCamera.Follow = player.transform;

        _playerAbilitySystem = player.GetComponent<PlayerAbilitySystem>();
        _playerCore = player.GetComponent<PlayerCore>();
        _playerInventory = player.GetComponent<PlayerInventorySystem>();
        _playerCraftingSystem = player.GetComponent<PlayerCraftingSystem>();
        _playerHealthSystem = player.GetComponent<AgentHealthSystem>();
    }

    private void InitializHotbar()
    {
        _playerHotbarDisplay.Initialize(_playerInventory, _playerHealthSystem);
    }

    private void InitializePlayerCraftingWindowDisplay()
    {
        _playerCraftingWindowDisplay.Initialize(_playerCraftingSystem, _playerInventory);
    }

    private void InitializeMouseItemSlot()
    {
        _mouseItemSlot.Initialize(_playerCore);
    }

    private void InitializePlayereHealthbar()
    {
        _playerHealthbarUI.Initialize(_playerHealthSystem);
    }

    private void InitializeAbilityCooldownUIDisplay()
    {
        _abilityCooldownUIDisplay.Initialize(_playerAbilitySystem);
    }

    private void InitializeGameOverScreen()
    {
        _gameOverScreen.Initialize(_playerHealthSystem);
    }

    private void InitializePreloadScreen()
    {
        _preloadScreen.Initialize();
    }

    private void InitializeTestSaveLoad()
    {
        _testSaveLoad.Initialize(_dataPersistanceManager);
    }
}