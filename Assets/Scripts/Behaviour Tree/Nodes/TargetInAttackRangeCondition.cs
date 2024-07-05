using UnityEngine;

public class TargetInAttackRangeCondition : Node
{

    public TargetInAttackRangeCondition(BaseEnemyBehaviour enemyBehaviour, float maxAttackRange, int nodePriority = 0) : base(nodePriority)
    {
        _maxAttackRange = maxAttackRange;
        _enemyBehaviour = enemyBehaviour;
    }

    private float _maxAttackRange;
    private BaseEnemyBehaviour _enemyBehaviour;


    public override ENodeState Evaluate()
    {
        return Vector2.Distance(_enemyBehaviour.transform.position,
                                _enemyBehaviour.GetCurrentTargetTransform().position) <= _maxAttackRange ? ENodeState.Success : ENodeState.Failure;
    }
}
