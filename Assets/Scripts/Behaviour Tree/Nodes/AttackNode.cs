using UnityEngine;

public class AttackNode : Node
{
    public AttackNode(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
    }

    private BaseEnemyBehaviour _enemyBehaviour;

    public override ENodeState Evaluate()
    {
        _enemyBehaviour.SetMovementDirection(Vector2.zero);
        _enemyBehaviour.SetAimPosition(_enemyBehaviour.GetCurrentTargetTransform().position);

        return ENodeState.Running;
    }
}
