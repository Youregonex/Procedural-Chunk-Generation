using System.Collections.Generic;
using UnityEngine;

public class WorldItemSpawner : MonoBehaviour
{
    public static WorldItemSpawner Instance { get; private set; }

    [Header("Debug Fields")]
    [SerializeField] private List<Item> _itemList;

    private ItemFactory _itemFactory = new ItemFactory();

    public List<Item> WorldItemList => _itemList;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    private void OnDestroy()
    {
        foreach (Item item in _itemList)
        {
            item.OnDestruction -= Item_OnDestruction;
        }
    }

    public Item SpawnItem(ItemDataSO itemDataSO, int quantity = 1)
    {
        Item item = _itemFactory.CreateItem(itemDataSO, quantity);
        _itemList.Add(item);
        item.OnDestruction += Item_OnDestruction;

        return item;
    }

    public Item SpawnNodeItem(ItemDataSO itemDataSO, int quantity = 1) // Spawns item for resource node without adding it to list
    {
        Item item = _itemFactory.CreateItem(itemDataSO, quantity);

        return item;
    }

    public void AddItem(Item item)
    {
        _itemList.Add(item);
        item.OnDestruction += Item_OnDestruction;
    }

    private void Item_OnDestruction(Item item)
    {
        if (!_itemList.Contains(item))
        {
            Debug.LogError($"Destroyed item wasn't tracked!");
            return;
        }

        item.OnDestruction -= Item_OnDestruction;
        _itemList.Remove(item);
    }
}