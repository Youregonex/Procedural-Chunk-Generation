using UnityEngine;

public class EnemyChaseState : BaseState<EnemyStateMachine.EEnemyState>
{
    public EnemyChaseState(
        EnemyStateMachine.EEnemyState key,
        EnemyStateMachine parentStateMachine,
        float attackRangeMax
        ) : base(key)
    {
        _parentStateMachine = parentStateMachine;
        _attackRangeMax = attackRangeMax;
    }

    private EnemyStateMachine _parentStateMachine;
    private float _attackRangeMax;

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (_parentStateMachine.GetCurrentTargetTransform() == null)
            return EnemyStateMachine.EEnemyState.Idle;

        if (Vector2.Distance(_parentStateMachine.GetPosition(), _parentStateMachine.GetCurrentTargetTransform().position) <= _attackRangeMax)
            return EnemyStateMachine.EEnemyState.Attack;

        return StateKey;
    }

    public override void UpdateState()
    {
        if (_parentStateMachine.GetCurrentTargetTransform() == null)
            return;

        _parentStateMachine.SetMovementDirection((Vector2)_parentStateMachine.GetCurrentTargetTransform().position - _parentStateMachine.GetPosition());
        _parentStateMachine.SetAimPosition(_parentStateMachine.GetCurrentTargetTransform().position);
    }
}
