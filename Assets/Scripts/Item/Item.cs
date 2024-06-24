using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour
{
    [SerializeField] private ItemDataSO _itemDataSO;
    public ItemDataSO ItemDataSO => _itemDataSO;

    [SerializeField] private int _itemQuantity;
    public int ItemQuantity => _itemQuantity;

    public void DestroyItem()
    {
        Destroy(gameObject);
    }

    public void ChangeItemQuantity(int newQuantity)
    {
        _itemQuantity = newQuantity;
    }

    public void SetItemData(ItemDataSO itemDataSO, int itemQuantity)
    {
        _itemDataSO = itemDataSO;
        _itemQuantity = itemQuantity;
    }
}
