
public class AgentSpawnedCondition : Node
{
    public AgentSpawnedCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
    }

    private BaseEnemyBehaviour _enemyBehaviour;

    public override ENodeState Evaluate()
    {
        return _enemyBehaviour.IsSpawned ? ENodeState.Success : ENodeState.Failure;
    }
}