using System.Collections.Generic;
using UnityEngine;

public class WorldItemSpawner : MonoBehaviour, IDataPersistance
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

    public void SaveData(ref GameData gameData)
    {
        List<ItemSaveData> itemSaveDataList = new List<ItemSaveData>();

        for (int i = 0; i < _itemList.Count; i++)
        {
            ItemSaveData itemSaveData = _itemList[i].GenerateSaveData() as ItemSaveData;
            itemSaveDataList.Add(itemSaveData);
        }

            gameData.itemSaveDataList = itemSaveDataList;
    }

    public void LoadData(GameData gameData)
    {
        _itemList = new List<Item>();

        for (int i = 0; i < gameData.itemSaveDataList.Count; i++)
        {
            Item item = SpawnItem(gameData.itemSaveDataList[i].itemDataSO);
            item.LoadFromSaveData(gameData.itemSaveDataList[i]);
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