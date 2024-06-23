using UnityEngine;

public class PlayerItemInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInventorySystem _playerInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            _playerInventory.AddItemToInventory(item);

            if(item.GetItemQuantity() <= 0)
            {
                item.DestroyItem();
            }
        }
    }
}
