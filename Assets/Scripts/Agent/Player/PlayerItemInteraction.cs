using UnityEngine;

public class PlayerItemInteraction : MonoBehaviour
{
    [SerializeField] private PlayerInventorySystem _playerInventory;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();

        if (item != null)
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
