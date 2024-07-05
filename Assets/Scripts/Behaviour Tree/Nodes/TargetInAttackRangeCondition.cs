using UnityEngine;

public class TargetInAttackRangeCondition : Node
{

    public TargetInAttackRangeCondition(BaseEnemyBehaviour enemyBehaviour, float maxAttackRange, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
        _maxAttackRange = maxAttackRange;
    }

    private BaseEnemyBehaviour _enemyBehaviour;
    private float _maxAttackRange;


    public override ENodeState Evaluate()
    {
        _nodeState = Vector2.Distance(_enemyBehaviour.transform.position,
                                      _enemyBehaviour.GetCurrentTargetTransform().position) <= _maxAttackRange ?
                                                                                                                ENodeState.Success :
                                                                                                                ENodeState.Failure;
        return _nodeState;
    }
}
