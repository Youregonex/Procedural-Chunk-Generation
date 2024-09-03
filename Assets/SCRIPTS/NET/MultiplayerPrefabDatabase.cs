using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPrefabDatabase : MonoBehaviour
{
    public static MultiplayerPrefabDatabase Instance;

    [Header("Config")]
    [SerializeField] private List<ResourceNode> _resourceNodePrefabList;
    [SerializeField] private List<ItemDataSO> _itemDataSOList;

    public List<ItemDataSO> ItemDataSOList => _itemDataSOList;

    private void Awake()
    {
        Instance = this;
    }

    public ResourceNode GetResourceNodePrefabWithId(int id)
    {
        if (id > _resourceNodePrefabList.Count || id < 0)
            return null;

        return _resourceNodePrefabList[id];
    }

    public int GetIdWithResourceNodePrefab(ResourceNode resourceNode)
    {
        return _resourceNodePrefabList.IndexOf(resourceNode);
    }

    public ItemDataSO GetItemDataSOWithId(int id)
    {
        if(id > _itemDataSOList.Count || id < 0)
            return null;

        return _itemDataSOList[id];
    }

    public int GetIdWithItemDataSO(ItemDataSO itemDataSO)
    {
        return _itemDataSOList.IndexOf(itemDataSO);
    }
}

