using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLootOnDestruction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DropList _nodeResourceDrop;

    [Header("Debug Fields")]
    [SerializeField] private List<Item> _lootList = new List<Item>();

    private void Awake()
    {
        StartCoroutine(GenerateResources());
    }

    private void OnDestroy()
    {
        DropLoot();
    }

    private IEnumerator GenerateResources()
    {
        Dictionary<ItemDropDataStruct, int> dropTable = GetDropTable();

        ItemFactory itemFactory = new ItemFactory();

        foreach (KeyValuePair<ItemDropDataStruct, int> keyValuePair in dropTable)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                Item item = itemFactory.CreateItemAtPosition(keyValuePair.Key.dropResource, transform.position);
                item.transform.SetParent(transform);
                _lootList.Add(item);
                item.gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    private Dictionary<ItemDropDataStruct, int> GetDropTable()
    {
        Dictionary<ItemDropDataStruct, int> dropTable = new Dictionary<ItemDropDataStruct, int>();

        foreach (ItemDropDataStruct resourceDrop in _nodeResourceDrop.ItemDropList)
        {
            if (Random.Range(0f, 1f) > resourceDrop.dropChance)
                continue;

            dropTable.Add(resourceDrop, Random.Range(resourceDrop.dropResourceAmountMin, resourceDrop.dropResourceAmountMax + 1));
        }

        return dropTable;
    }

    private void DropLoot()
    {
        if (_lootList.Count == 0)
            return;

        foreach (Item item in _lootList)
        {
            item.transform.SetParent(null);
            item.gameObject.SetActive(true);
            item.enabled = true;
            item.Drop();
        }

        _lootList.Clear();
    }
}
