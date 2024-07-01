using UnityEngine;

public class EnemyRoamState : BaseState<EnemyStateMachine.EEnemyState>
{
    public EnemyRoamState(
        EnemyStateMachine.EEnemyState key,
        EnemyStateMachine parentStateMachine
        ) : base(key)
    {
        _parentStateMachine = parentStateMachine;
    }

    private EnemyStateMachine _parentStateMachine;

    private Vector2 _currentRoamPosition;
    private float _distanceThreshold = .1f;
    private float _timeThreshold = 5f;



    public override void EnterState()
    {
        base.EnterState();

        _currentRoamPosition = _parentStateMachine.GetCurrentRoamPosition();
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (_parentStateMachine.TargetTransformList.Count != 0)
            return EnemyStateMachine.EEnemyState.Chase;

        if (Vector2.Distance(_parentStateMachine.GetPosition(), _currentRoamPosition) <= _distanceThreshold)
            return EnemyStateMachine.EEnemyState.Idle;

        if(StateCurrentTime >= _timeThreshold)
            return EnemyStateMachine.EEnemyState.Idle;

        return StateKey;
    }

    public override void UpdateState()
    {
        _parentStateMachine.SetMovementDirection(_currentRoamPosition - _parentStateMachine.GetPosition());
        _parentStateMachine.SetAimPosition(_currentRoamPosition);
    }

    public override void ExitState()
    {
        _currentRoamPosition = Vector2.zero;
    }
}
