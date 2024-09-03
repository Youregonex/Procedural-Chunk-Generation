using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DropLootOnDestruction : NetworkBehaviour
{
    [Header("Config")]
    [SerializeField] private DropListDataSO _nodeResourceDrop;

    [Header("Debug Fields")]
    [SerializeField] private List<Item> _lootList = new();

    private IContainLoot _lootContainer;

    private void Awake()
    {
        _lootContainer = GetComponent<IContainLoot>();
        _lootContainer.OnLootDrop += IContainLoot_OnLootDrop;        
    }

    private void IContainLoot_OnLootDrop()
    {
        SpawnLootServerRpc();
    }

    private IEnumerator GenerateLootList()
    {
        Dictionary<ItemDataSO, int> dropTable = LootTableGenerator.GetDropTable(_nodeResourceDrop);

        foreach (KeyValuePair<ItemDataSO, int> keyValuePair in dropTable)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                if (WorldItemSpawner.Instance == null)
                    yield break;

                Item item = WorldItemSpawner.Instance.SpawnItem(keyValuePair.Key);
                item.NetworkObject.Spawn();
                item.transform.position = transform.position;

                _lootList.Add(item);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnLootServerRpc()
    {
        StartCoroutine(DropLoot());
    }

    private IEnumerator DropLoot()
    {
        yield return GenerateLootList();

        if (_lootList.Count == 0)
            yield break;

        foreach (Item item in _lootList)
        {
            item.enabled = true;
            item.DropInRandomDirection();
        }

        _lootList.Clear();
    }
}
