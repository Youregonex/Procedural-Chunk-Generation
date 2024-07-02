using UnityEngine;

public class ItemFactory
{
    public Item CreateItem(ItemDataSO itemDataSO, int quantity)
    {
        Transform itemTransform = GameObject.Instantiate(itemDataSO.Prefab);

        Item item = itemTransform.GetComponent<Item>();
        item.SetItemData(itemDataSO, quantity);

        return item;
    }
}
