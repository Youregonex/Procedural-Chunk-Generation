using UnityEngine;
using Cinemachine;
using System.Collections;
using Unity.Netcode;

public class GameSceneInitializer : MonoBehaviour
{
    public static GameSceneInitializer Instance;

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
    [SerializeField] private TestSaveLoad _testSaveLoad;

    [Header("Debug Fields")]
    [SerializeField] private GameScenePreloader _gameScenePreloader;
    [SerializeField] private PlayerInventorySystem _playerInventory;
    [SerializeField] private PlayerCraftingSystem _playerCraftingSystem;
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private AgentStatHealthSystem _playerHealthSystem;
    [SerializeField] private PlayerAbilitySystem _playerAbilitySystem;
    [SerializeField] private PlayerData _playerData;
    [SerializeField, Space(10)] private bool _testMode;

    private void Awake()
    {
        Instance = this;
    }

    public void SetupScene(GameScenePreloader gameScenePreloader)
    {
        StartCoroutine(SetupSceneCoroutine(null, gameScenePreloader));
    }

    public void StartSceneSetup(PlayerCore playerCore)
    {
        if (_testMode)
        {
            StartCoroutine(SetupSceneCoroutine(playerCore));
        }
    }

    private IEnumerator SetupSceneCoroutine(PlayerCore playerCore = null, GameScenePreloader gameScenePreloader = null)
    {
        _gameScenePreloader = gameScenePreloader;

        yield return InitialSceneSetup(playerCore);

        if (DataPersistanceManager.Instance != null && DataPersistanceManager.Instance.IsLoadingGame)
        {
            DataPersistanceManager.Instance.LoadGame();
        }
        else
        {
        }

        if(_gameScenePreloader != null)
            _gameScenePreloader.FinishPreloading();
    }

    private IEnumerator InitialSceneSetup(PlayerCore playerCore)
    {
        SetupPlayer(playerCore);

        InitializHotbarDisplay();
        InitializePlayerCraftingWindowDisplay();
        InitializeMouseItemSlot();
        InitializePlayerHealthSystem();
        InitializePlayereHealthbarUI();
        InitializeAbilityCooldownUIDisplay();
        InitializeGameOverScreen();

        InitializePlayerAbilitySystem();
        InitializeTestSaveLoad();

        InitializePlayerData();

        yield return null;
    }

    private void SetupPlayer(PlayerCore playerCore)
    {
        if (playerCore == null)
        {
            GameObject player = Instantiate(_playerPrefab, _playerSpawn.position, Quaternion.identity);
            _playerCore = player.GetComponent<PlayerCore>();
        }
        else
        {
            _playerCore = playerCore;
            _playerCore.gameObject.name = $"Player {NetworkManager.Singleton.LocalClientId}";
        }

        _playerFollowCamera.Follow = _playerCore.transform;

        _playerAbilitySystem = _playerCore.GetAgentComponent<PlayerAbilitySystem>();
        _playerInventory = _playerCore.GetAgentComponent<PlayerInventorySystem>();
        _playerCraftingSystem = _playerCore.GetAgentComponent<PlayerCraftingSystem>();
        _playerHealthSystem = _playerCore.GetAgentComponent<AgentStatHealthSystem>();
        _playerData = _playerCore.GetAgentComponent<PlayerData>();
    }

    private void InitializePlayerAbilitySystem() // Initialize after AbilityCooldownDisplay
    {
        _playerAbilitySystem.Initialize();
    }

    private void InitializePlayerHealthSystem() // Initialize before PlayerHealthbarUI
    {
        _playerHealthSystem.Initialize();
    }

    private void InitializHotbarDisplay()
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

    private void InitializePlayereHealthbarUI()
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

    private void InitializeTestSaveLoad()
    {
        _testSaveLoad.Initialize();
    }

    private void InitializePlayerData()
    {
        _playerData.Initialize();
    }
}