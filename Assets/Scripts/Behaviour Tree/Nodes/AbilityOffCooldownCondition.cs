
public class AbilityOffCooldownCondition : Node
{
    private string _abilityName;
    private AgentAbilitySystem _agentAbilitySystem;

    public AbilityOffCooldownCondition(AgentAbilitySystem agentAbilitySystem, string abilityName, int nodePriority = 0) : base(nodePriority)
    {
        _agentAbilitySystem = agentAbilitySystem;
        _abilityName = abilityName;
    }

    public override ENodeState Evaluate()
    {
        return _agentAbilitySystem.IsOnCooldown(_abilityName) ? ENodeState.Failure : ENodeState.Success;
    }
}
