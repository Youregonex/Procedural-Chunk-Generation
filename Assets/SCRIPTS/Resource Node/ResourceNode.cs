using UnityEngine;
using System;

[Serializable]
public class ResourceNode : MonoBehaviour, IContainLoot
{
    public event Action<Vector2Int> OnDepletion;
    public event Action OnLootDrop;

    [Header("Config")]
    [SerializeField] private EToolType _appropriateTool;
    [SerializeField] private DropListDataSO _dropListDataSO;

    [Header("Debug Fields")]
    [SerializeField] private ResourceNodeHealthSystem _nodeHealthSystem;

    public EToolType AppropriateTool => _appropriateTool;

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

    public void OnLootDropInvoke()
    {
        OnLootDrop?.Invoke();
    }

    private void NodeHealthSystem_OnDeath()
    {
        OnLootDrop?.Invoke();
        OnDepletion?.Invoke(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }
}