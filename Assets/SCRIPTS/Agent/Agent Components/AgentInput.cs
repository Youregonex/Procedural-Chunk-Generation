using UnityEngine;
using System;

public abstract class AgentInput : AgentMonobehaviourComponent
{
    public event EventHandler OnAgentAttackTriggered;

    public abstract Vector2 GetMovementVectorNormalized();
    public abstract Vector2 GetAimPosition();

    public virtual Vector2 GetAimPositionNormalized() => GetAimPosition().normalized;

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    protected void Invoke_AgentInput_OnAgentAttackTriggered()
    {
        OnAgentAttackTriggered?.Invoke(this, EventArgs.Empty);
    }
}
