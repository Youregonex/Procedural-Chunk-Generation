using UnityEngine;
using Unity.Netcode;

public abstract class AgentNetworkBehaviourComponent : NetworkBehaviour
{
    [field: Header("Config")]
    [field: SerializeField] public bool IsRootComponent { get; protected set; }
    [field: SerializeField] public bool DisableOnDeath { get; protected set; }

    [field: Header("Debug Fields")]
    [field: SerializeField] public AgentCoreBase AgentCore { get; protected set; }

    public virtual void GetAgentCore()
    {
        if (IsRootComponent)
            AgentCore = GetComponent<AgentCoreBase>();
        else
            AgentCore = transform.root.GetComponent<AgentCoreBase>();
    }


    public abstract void Initialize();
    public abstract void DisableComponent();
    public abstract void EnableComponent();

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            enabled = false;
            return;
        }
    }
}
