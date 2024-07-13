using UnityEngine;

public class IsCastingCondition : Node
{
    private AgentAbilitySystem _agentAbilitySystem;

    public IsCastingCondition(AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
    {
        _agentAbilitySystem = agentAbilitySystem;
    }

    public override ENodeState Evaluate()
    {
        return _agentAbilitySystem.IsCastingAbility ? ENodeState.Success : ENodeState.Failure;
    }
}
