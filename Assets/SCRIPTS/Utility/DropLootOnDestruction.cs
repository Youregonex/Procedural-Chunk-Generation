using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLootOnDestruction : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private DropListDataSO _nodeResourceDrop;

    [Header("Debug Fields")]
    [SerializeField] private List<Item> _lootList = new List<Item>();

    private IContainLoot _lootContainer;

    private void Awake()
    {
        _lootContainer = GetComponent<IContainLoot>();
        _lootContainer.OnLootDrop += IContainLoot_OnLootDrop;
        
        StartCoroutine(GenerateLootList());
    }

    private void IContainLoot_OnLootDrop()
    {
        DropLoot();
    }

    private IEnumerator GenerateLootList()
    {
        Dictionary<ItemDataSO, int> dropTable = LootTableGenerator.GetDropTable(_nodeResourceDrop);

        foreach (KeyValuePair<ItemDataSO, int> keyValuePair in dropTable)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                Item item = WorldItemSpawner.Instance.SpawnNodeItem(keyValuePair.Key);
                item.transform.position = transform.position;
                item.transform.SetParent(transform);
                _lootList.Add(item);
                item.gameObject.SetActive(false);
            }

            yield return null;
        }

        _lootContainer.FillLootList(_lootList);
    }

    private void DropLoot()
    {
        if (_lootList.Count == 0)
            return;

        foreach (Item item in _lootList)
        {
            WorldItemSpawner.Instance.AddItem(item);

            item.transform.SetParent(null);
            item.gameObject.SetActive(true);
            item.enabled = true;
            item.DropInRandomDirection();
        }

        _lootList.Clear();
    }
}
