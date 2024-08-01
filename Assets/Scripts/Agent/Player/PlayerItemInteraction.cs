using UnityEngine;

public class PlayerItemInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInventorySystem _playerInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Item item))
        {
            int amountDidntFit = _playerInventory.AddItemToInventory(item);

            if (amountDidntFit == 0)
            {
                item.DestroyItem();
            }
            else
            {
                item.ChangeItemQuantity(amountDidntFit);
            }
        }
    }
}
