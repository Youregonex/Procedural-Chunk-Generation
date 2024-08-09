using UnityEngine;
using UnityEngine.InputSystem;
using Youregone.Utilities;

public class PlayerBuildingSystem : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] private PendingBuildingItem _pendingBuildingItem;
    [field: SerializeField] private float _buildingRange = 3f;
    
    [Header("Debug Fields")]
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private PlayerItemSelection _playerItemSelection;
    [SerializeField] private BuildingItemDataSO _currentBuildingItemDataSO;

    private Camera _mainCamera;

    private void Awake()
    {
        _playerCore = GetComponent<PlayerCore>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _playerItemSelection = _playerCore.GetAgentComponent<PlayerItemSelection>();
        _playerItemSelection.OnCurrentItemChanged += PlayerItemSelection_OnCurrentItemChanged;
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBuilding();
        }
    }

    private void OnDestroy()
    {
        _playerItemSelection.OnCurrentItemChanged -= PlayerItemSelection_OnCurrentItemChanged;
    }

    public override void DisableComponent()
    {
        _pendingBuildingItem.HidePendingBuildingItem();
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void PlayerItemSelection_OnCurrentItemChanged(ItemDataSO itemDataSO)
    {
        BuildingItemDataSO buildingItemDataSO = itemDataSO as BuildingItemDataSO;

        if(buildingItemDataSO != null)
        {
            _currentBuildingItemDataSO = buildingItemDataSO;
            RefreshPendingBuildingItem(buildingItemDataSO);
        }
        else
        {
            HidePendingBuildingItem();
            _currentBuildingItemDataSO = null;
        }
    }

    private void PlaceBuilding()
    {
        if (_currentBuildingItemDataSO != null && _pendingBuildingItem.CanPlaceBuilding)
        {
            WorldBuildingSpawner.Instance.CreateBuildingAtPosition(_currentBuildingItemDataSO, Utility.GetMouseGridPosition());
            _currentBuildingItemDataSO.OnBuildingItemPlacedInvoke();
        }
    }

    private void RefreshPendingBuildingItem(BuildingItemDataSO buildingItemDataSO)
    {
        _pendingBuildingItem.RefreshPendingBuildingItem(buildingItemDataSO, _buildingRange, transform);
    }

    private void HidePendingBuildingItem()
    {
        _pendingBuildingItem.HidePendingBuildingItem();
    }
}
