using UnityEngine;

public class TargetInRangeCondition : Node
{
    private BaseEnemyBehaviour _enemyBehaviour;
    private float _range;

    public TargetInRangeCondition(BaseEnemyBehaviour enemyBehaviour, float range, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
        _range = range;
    }

    public override ENodeState Evaluate()
    {
        _nodeState = Vector2.Distance(_enemyBehaviour.transform.position,
                                      _enemyBehaviour.GetCurrentTargetTransform().position) <= _range ?
                                                                                                       ENodeState.Success :
                                                                                                       ENodeState.Failure;
        return _nodeState;
    }
}
