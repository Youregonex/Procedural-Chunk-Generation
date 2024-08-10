using UnityEngine;
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
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private bool _buildingEnabled = false;

    private void Awake()
    {
        _playerCore = GetComponent<PlayerCore>();
    }

    private void Start()
    {
        _playerItemSelection = _playerCore.GetAgentComponent<PlayerItemSelection>();
        _playerItemSelection.OnCurrentItemChanged += PlayerItemSelection_OnCurrentItemChanged;

        _playerInput = _playerCore.GetAgentComponent<PlayerInput>();
        _playerInput.OnMousePrimary += PlayerInput_OnMousePrimary;
    }

    private void OnDestroy()
    {
        _playerItemSelection.OnCurrentItemChanged -= PlayerItemSelection_OnCurrentItemChanged;
        _playerInput.OnMousePrimary -= PlayerInput_OnMousePrimary;
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

    private void PlayerInput_OnMousePrimary(object sender, System.EventArgs e)
    {
        if(_buildingEnabled)
            PlaceBuilding();
    }

    private void PlayerItemSelection_OnCurrentItemChanged(ItemDataSO itemDataSO)
    {
        BuildingItemDataSO buildingItemDataSO = itemDataSO as BuildingItemDataSO;

        if(buildingItemDataSO != null)
        {
            _buildingEnabled = true;
            _currentBuildingItemDataSO = buildingItemDataSO;
            RefreshPendingBuildingItem(buildingItemDataSO);
        }
        else
        {
            HidePendingBuildingItem();
            _buildingEnabled = false;
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