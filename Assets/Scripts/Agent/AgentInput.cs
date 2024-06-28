using UnityEngine;
using System;

public abstract class AgentInput : MonoBehaviour
{
    public event EventHandler OnAgentAttackTriggered;

    public abstract Vector2 GetMovementVectorNormalized();
    public abstract Vector2 GetAimPosition();

    public virtual Vector2 GetAimPositionNormalized() => GetAimPosition().normalized;

    protected virtual void Invoke_OnAgentAttackTriggered()
    {
        OnAgentAttackTriggered?.Invoke(this, EventArgs.Empty);
    }
}
