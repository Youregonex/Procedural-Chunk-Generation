using UnityEngine;

public class RoamPositionExistsCondition : Node
{
    public RoamPositionExistsCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
    }

    private BaseEnemyBehaviour _enemyBehaviour;

    public override ENodeState Evaluate()
    {
        return _enemyBehaviour.CurrentRoamPosition != Vector2.zero ? ENodeState.Success : ENodeState.Failure;
    }
}
