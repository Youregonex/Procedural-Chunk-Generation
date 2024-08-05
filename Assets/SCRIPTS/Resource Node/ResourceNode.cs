using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceNode : MonoBehaviour, IContainLoot
{
    public event Action<Vector2Int> OnDepletion;
    public event Action OnLootDrop;

    [Header("Config")]
    [SerializeField] private EToolType _appropriateTool;
    [SerializeField] private DropListDataSO _dropListDataSO;

    [Header("Debug Fields")]
    [SerializeField] private ResourceNodeHealthSystem _nodeHealthSystem;
    [SerializeField] private List<Item> _lootList;

    public EToolType AppropriateTool => _appropriateTool;

    public IEnumerable<Item> LootList => _lootList;

    private void Awake()
    {
        _nodeHealthSystem = GetComponent<ResourceNodeHealthSystem>();
    }

    private void Start()
    {
        _nodeHealthSystem.OnDeath += NodeHealthSystem_OnDeath;
    }

    private void OnDestroy()
    {
        _nodeHealthSystem.OnDeath -= NodeHealthSystem_OnDeath;
    }

    public void FillLootList(List<Item> lootList)
    {
        _lootList = lootList;
    }

    private void NodeHealthSystem_OnDeath()
    {
        OnLootDrop?.Invoke();
        OnDepletion?.Invoke(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }
}