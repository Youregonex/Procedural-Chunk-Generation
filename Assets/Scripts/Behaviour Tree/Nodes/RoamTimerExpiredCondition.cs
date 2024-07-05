using UnityEngine;

public class RoamTimerExpiredCondition : Node
{
    public RoamTimerExpiredCondition(BaseEnemyBehaviour enemyBehaviour, float timer, int nodePriority = 0) : base(nodePriority)
    {
        _enemyBehaviour = enemyBehaviour;
        _timerMax = timer;
    }

    private BaseEnemyBehaviour _enemyBehaviour;
    private float _timerMax;
    private float _timerCurrent = 0;

    public override ENodeState Evaluate()
    {
        Debug.Log($"Current Timer : {_timerCurrent}");

        if (_timerCurrent >= _timerMax)
        {
            _timerCurrent = 0;
            _enemyBehaviour.SetRoamPosition(Vector2.zero);
            _nodeState = ENodeState.Success;
        }
        else
        {
            _timerCurrent += Time.deltaTime;
            _nodeState = ENodeState.Failure;
        }

        return _nodeState;
    }
}
