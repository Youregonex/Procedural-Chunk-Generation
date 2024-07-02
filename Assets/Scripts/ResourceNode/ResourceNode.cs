using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private ResourceNodeDropDataSO _nodeResourceDrop;
    [SerializeField] private EToolType _appropriateTool;

    public EToolType AppropriateTool => _appropriateTool;

    private void Awake()
    {
        
    }

    private void OnDestroy()
    {

    }
}
