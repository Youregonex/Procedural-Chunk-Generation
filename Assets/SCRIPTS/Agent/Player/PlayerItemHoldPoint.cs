using Unity.Netcode;
using UnityEngine;

public class PlayerItemHoldPoint : AgentItemHoldPoint
{
    [Header("Config")]
    [SerializeField] private SpriteRenderer _itemSpriteRenderer;

    [Header("Debug Fields")]
    [SerializeField] private ItemDataSO _currentItemDataSO;
    [SerializeField] private PlayerItemSelection _playerItemSelection;


    public override void Initialize()
    {
        base.Initialize();

        _playerItemSelection = AgentCore.GetAgentComponent<PlayerItemSelection>();
        _playerItemSelection.OnCurrentItemChanged += PlayerItemSelection_OnCurrentItemChanged;
    }

    public override void OnDestroy()
    {
        if(_playerItemSelection != null)
            _playerItemSelection.OnCurrentItemChanged -= PlayerItemSelection_OnCurrentItemChanged;
    }

    private void SetItemHoldPointData(ItemDataSO itemDataSO)
    {
        _currentItemDataSO = itemDataSO;
        _itemSpriteRenderer.sprite = itemDataSO.Icon;
    }

    private void ClearItemHoldPointData()
    {
        _currentItemDataSO = null;
        _itemSpriteRenderer.sprite = null;
    }

    private void PlayerItemSelection_OnCurrentItemChanged(ItemDataSO itemDataSO)
    {
        if (itemDataSO == null || itemDataSO.ItemType == EItemType.Weapon || itemDataSO.ItemType == EItemType.Tool)
        {
            ClearItemHoldPointData();
            UpdateCurrentItemVisualClientRpc(this.NetworkObject, -1, RpcTarget.Not(NetworkObject.OwnerClientId, RpcTargetUse.Temp));
        }
        else
        {
            SetItemHoldPointData(itemDataSO);

            int itemDataSOId = MultiplayerPrefabDatabase.Instance.GetIdWithItemDataSO(itemDataSO);
            UpdateCurrentItemVisualClientRpc(this.NetworkObject, itemDataSOId, RpcTarget.Not(NetworkObject.OwnerClientId, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void UpdateCurrentItemVisualClientRpc(NetworkObjectReference playerItemHoldPointNetworkObjectReference, int itemDataSOId, RpcParams rpcParams)
    {
        if(playerItemHoldPointNetworkObjectReference.TryGet(out NetworkObject playerItemHoldPointnetworkObject))
        {
            if(playerItemHoldPointnetworkObject.TryGetComponent(out PlayerItemHoldPoint playerItemHoldPoint))
            {
                if(playerItemHoldPoint == null)
                {
                    Debug.Log("PlayerItemHoldPoint wasn't found!");
                    return;
                }

                ItemDataSO newItemDataSO = MultiplayerPrefabDatabase.Instance.GetItemDataSOWithId(itemDataSOId);

                if (newItemDataSO == null || newItemDataSO.ItemType == EItemType.Weapon || newItemDataSO.ItemType == EItemType.Tool)
                    playerItemHoldPoint.ClearItemHoldPointData();
                else
                    playerItemHoldPoint.SetItemHoldPointData(newItemDataSO);
            }
            else
            {
                Debug.LogError("Couldn't get PlayerItemHoldPoint out of PlayerItemHoldPointNetworkObjectReference");
            }
        }
        else
        {
            Debug.LogError("Couldn't unpack PlayerNetworkObjectReference");
        }
    }
}
