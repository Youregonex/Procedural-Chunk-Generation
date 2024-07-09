
public class CastDashNode : Node
{
    private string _abilityName;
    private AgentAbilitySystem _agentAbilitySystem;
    private BaseEnemyBehaviour _enemyBehaviour;

    public CastDashNode(BaseEnemyBehaviour enemyBehaviour, string abilityName, AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
        _abilityName = abilityName;
        _agentAbilitySystem = agentAbilitySystem;
    }

    public override ENodeState Evaluate()
    {
        _agentAbilitySystem.CastAbility(_abilityName, _enemyBehaviour.MovementDirection);
        return ENodeState.Running;
    }
}
