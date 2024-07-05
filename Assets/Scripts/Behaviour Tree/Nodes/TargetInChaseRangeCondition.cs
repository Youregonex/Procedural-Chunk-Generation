using UnityEngine;

public class TargetInChaseRangeCondition : Node
{
    public TargetInChaseRangeCondition(BaseEnemyBehaviour enemyBehaviour, float chaseRange, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
        _chaseRange = chaseRange;
    }

    private BaseEnemyBehaviour _enemyBehaviour;
    private float _chaseRange;


    public override ENodeState Evaluate()
    {
        _nodeState = Vector2.Distance(_enemyBehaviour.transform.position,
                                      _enemyBehaviour.GetCurrentTargetTransform().position) <= _chaseRange ?
                                                                                                           ENodeState.Success :
                                                                                                           ENodeState.Failure;
        return _nodeState;
    }
}
