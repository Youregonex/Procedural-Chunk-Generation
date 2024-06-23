using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    [SerializeField] private ItemDataSO _itemDataSO;
    [SerializeField] private int _itemQuantity;

    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public ItemDataSO GetItemDataSO() => _itemDataSO;
    public int GetItemQuantity() => _itemQuantity;
}
