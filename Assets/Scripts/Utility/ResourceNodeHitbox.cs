using UnityEngine;

[RequireComponent(typeof(ResourceNodeHealthSystem))]
public class ResourceNodeHitbox : MonoBehaviour, IGatherable
{
    [Header("Config")]
    [SerializeField] private ResourceNodeHealthSystem _resourceNodeHealthSystem;

    [Header("Debug Fields")]
    [SerializeField] private ResourceNode _resourceNode;

    private void Start()
    {
        _resourceNodeHealthSystem = GetComponent<ResourceNodeHealthSystem>();
        _resourceNode = transform.root.GetComponent<ResourceNode>();
    }

    public bool IsDead() => _resourceNodeHealthSystem.IsDepleted;

    public void Gather(GatherStruct gatherStruct)
    {
        if (gatherStruct.toolType != _resourceNode.AppropriateTool)
            return;

        _resourceNodeHealthSystem.Gather(gatherStruct);
    }

    public bool IsDepleted() => _resourceNodeHealthSystem.IsDepleted;
}
