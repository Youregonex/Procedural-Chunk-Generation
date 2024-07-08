using UnityEngine;

public abstract class AgentMonobehaviourComponent : MonoBehaviour
{
    [field: SerializeField] public bool DisableOnDeath { get; protected set; }

    public abstract void DisableComponent();
}
