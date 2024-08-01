using UnityEngine;

public class ResourceNodeHitbox : MonoBehaviour, IGatherable
{
    [Header("Debug Fields")]
    [SerializeField] private ResourceNode _resourceNode;
    [SerializeField] private ResourceNodeHealthSystem _resourceNodeHealthSystem;

    private void Awake()
    {
        _resourceNodeHealthSystem = transform.root.GetComponent<ResourceNodeHealthSystem>();
        _resourceNode = transform.root.GetComponent<ResourceNode>();
    }

    public void Gather(GatherStruct gatherStruct)
    {
        if (gatherStruct.toolType != _resourceNode.AppropriateTool)
            return;

        _resourceNodeHealthSystem.Gather(gatherStruct);
    }

    public bool IsDepleted() => _resourceNodeHealthSystem.IsDepleted;
}
