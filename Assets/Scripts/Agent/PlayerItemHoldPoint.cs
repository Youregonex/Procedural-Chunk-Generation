using UnityEngine;

public class PlayerItemHoldPoint : ItemHoldPoint
{
    [Header("Config")]
    [SerializeField] private SpriteRenderer _itemSpriteRenderer;

    [Header("Debug Fields")]
    [SerializeField] private ItemDataSO _currentItemDataSO;
    [SerializeField] private PlayerItemSelection _playerItemSelection;

    protected override void Awake()
    {
        _agentCore = transform.root.GetComponent<PlayerCore>();
    }

    protected override void Start()
    {
        base.Start();

        _playerItemSelection = _agentCore.GetAgentComponent<PlayerItemSelection>();
        _playerItemSelection.OnCurrentItemChanged += PlayerItemSelection_OnCurrentItemChanged;
    }

    private void PlayerItemSelection_OnCurrentItemChanged(ItemDataSO itemDataSO)
    {
        if (itemDataSO == null || itemDataSO.ItemType == EItemType.Weapon || itemDataSO.ItemType == EItemType.Tool)
        {
            _currentItemDataSO = null;
            _itemSpriteRenderer.sprite = null;
            return;
        }

        _currentItemDataSO = itemDataSO;
        _itemSpriteRenderer.sprite = itemDataSO.Icon;
    }
}
