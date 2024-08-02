using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuildingSystem : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] private PendingBuildingItem _pendingBuildingItem;
    [field: SerializeField] private float _buildingRange = 3f;
    
    [Header("Debug Fields")]
    [SerializeField] private PlayerCore _playerCore;
    [SerializeField] private PlayerItemSelection _playerItemSelection;
    [SerializeField] private BuildingItemDataSO _currentBuildingItemDataSO;

    private BuildingFactory _buildingFactory;

    private void Awake()
    {
        _buildingFactory = new BuildingFactory();
        _playerCore = GetComponent<PlayerCore>();
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
            _buildingFactory.CreateBuildingAtPosition(_currentBuildingItemDataSO, GetMouseGridPosition());
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

    private Vector2 GetMouseGridPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        return new Vector2(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y));
    }
}
