using UnityEngine;

public class ItemFactory
{
    public Item CreateItem(ItemDataSO itemDataSO, int quantity = 1)
    {
        Transform itemTransform = GameObject.Instantiate(itemDataSO.ItemPrefab);

        Item item = itemTransform.GetComponent<Item>();
        item.SetItemData(itemDataSO, quantity);

        return item;
    }
}
