using UnityEngine;
using System;

public class ResourceNode : MonoBehaviour
{
    public event Action<Vector2Int> OnDepletion;

    [Header("Config")]
    [SerializeField] private EToolType _appropriateTool;

    [Header("Debug Fields")]
    [SerializeField] private ResourceNodeHealthSystem _nodeHealthSystem;

    public EToolType AppropriateTool => _appropriateTool;

    private void Awake()
    {
        _nodeHealthSystem = GetComponent<ResourceNodeHealthSystem>();
    }

    private void Start()
    {
        _nodeHealthSystem.OnDepletion += NodeHealthSystem_OnDepletion;
    }

    private void OnDestroy()
    {
        _nodeHealthSystem.OnDepletion -= NodeHealthSystem_OnDepletion;
    }

    private void NodeHealthSystem_OnDepletion()
    {
        OnDepletion?.Invoke(new Vector2Int((int)transform.position.x, (int)transform.position.y));
    }
}