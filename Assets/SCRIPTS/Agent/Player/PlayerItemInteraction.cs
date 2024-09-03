using UnityEngine;
using Unity.Netcode;

public class PlayerItemInteraction : NetworkBehaviour
{
    [SerializeField] private PlayerInventorySystem _playerInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Item item))
        {
            int amountDidntFit = _playerInventory.AddItemToInventory(item);

            if (amountDidntFit == 0)
            {
                DestroyItemServerRpc(item.NetworkObject);
            }
            else
            {
                ChangeItemQuantityClientRpc(item.NetworkObject, amountDidntFit);
            }
        }
    }

    [Rpc(SendTo.Server)]
    private void DestroyItemServerRpc(NetworkObjectReference itemNetworkObjectReference)
    {
        if(itemNetworkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Debug.Log("Despawning Item");
            Item item = networkObject.GetComponent<Item>();
            item.NetworkObject.Despawn();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ChangeItemQuantityClientRpc(NetworkObjectReference itemNetworkObjectReference, int amountLeft)
    {
        if (itemNetworkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Item item = networkObject.GetComponent<Item>();
            item.ChangeItemQuantity(amountLeft);
        }
    }
}
