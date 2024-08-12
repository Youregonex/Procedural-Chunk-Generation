using UnityEngine;
using Unity.Netcode;

public abstract class AgentMonoBehaviourComponent : NetworkBehaviour
{
    [field: SerializeField] public bool DisableOnDeath { get; protected set; }

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
