
public class TargetExistsCondition : Node
{
    public TargetExistsCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
    }

    private BaseEnemyBehaviour _enemyBehaviour;

    public override ENodeState Evaluate()
    {
        return _enemyBehaviour.GetCurrentTargetTransform() != null ? ENodeState.Success : ENodeState.Failure;
    }
}
