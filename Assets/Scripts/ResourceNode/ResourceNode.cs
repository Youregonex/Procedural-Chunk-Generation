using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceNode : MonoBehaviour
{
    public event Action<Vector2Int> OnDepletion;

    [Header("Config")]
    [SerializeField] private ResourceNodeDropDataSO _nodeResourceDrop;
    [SerializeField] private EToolType _appropriateTool;

    [Header("Debug Fields")]
    [SerializeField] private ResourceNodeHealthSystem _nodeHealthSystem;
    [SerializeField] private List<Item> _nodeResourcesList;

    public EToolType AppropriateTool => _appropriateTool;

    private void Awake()
    {
        _nodeHealthSystem = GetComponent<ResourceNodeHealthSystem>();

        StartCoroutine(GenerateResources());
    }

    private void Start()
    {
        _nodeHealthSystem.OnDepletion += NodeHealthSystem_OnDepletion;
    }

    private void OnDestroy()
    {
        _nodeHealthSystem.OnDepletion -= NodeHealthSystem_OnDepletion;
    }

    private IEnumerator GenerateResources()
    {
        Dictionary<NodeResourceDropDataStruct, int> dropTable = GetDropTable();

        ItemFactory itemFactory = new ItemFactory();
        Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-.2f, .2f), UnityEngine.Random.Range(-.2f, .2f));

        foreach(KeyValuePair<NodeResourceDropDataStruct, int> keyValuePair in dropTable)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                Item item = itemFactory.CreateItemAtPosition(keyValuePair.Key.dropResource, (Vector2)transform.position + randomOffset);
                item.gameObject.SetActive(false);
                item.transform.SetParent(transform);
                _nodeResourcesList.Add(item);
            }

            yield return null;
        }
    }

    private Dictionary<NodeResourceDropDataStruct, int> GetDropTable()
    {
        Dictionary<NodeResourceDropDataStruct, int> dropTable = new Dictionary<NodeResourceDropDataStruct, int>();

        foreach (NodeResourceDropDataStruct resourceDrop in _nodeResourceDrop.nodeResourceDropDataList)
        {
            if (UnityEngine.Random.Range(0f, 1f) > resourceDrop.dropChance)
                continue;

            dropTable.Add(resourceDrop, UnityEngine.Random.Range(resourceDrop.dropResourceAmountMin, resourceDrop.dropResourceAmountMax + 1));
        }

        return dropTable;
    }

    private void NodeHealthSystem_OnDepletion()
    {
        DropResources();

        OnDepletion?.Invoke(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }

    private void DropResources()
    {
        foreach (Item item in _nodeResourcesList)
        {
            item.transform.SetParent(null);
            item.gameObject.SetActive(true);

            item.Drop();
        }

        _nodeResourcesList.Clear();
    }
}