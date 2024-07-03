using UnityEngine;

public class NodeFlashOnHit : FlashOnHit
{
    [Header("Config")]
    [SerializeField] private ResourceNodeHealthSystem _nodeHealthSystem;

    private void Start()
    {
        _nodeHealthSystem.OnGather += NodeHealthSystem_OnGather;
    }

    private void OnDestroy()
    {
        _nodeHealthSystem.OnGather -= NodeHealthSystem_OnGather;
    }

    private void NodeHealthSystem_OnGather(GatherStruct gatherStruct)
    {
        StopAllCoroutines();

        StartCoroutine(FlashSprite());
    }
}
