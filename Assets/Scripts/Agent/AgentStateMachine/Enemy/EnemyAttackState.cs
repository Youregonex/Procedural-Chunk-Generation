using UnityEngine;

public class EnemyAttackState : BaseState<EnemyStateMachine.EEnemyState>
{
    public EnemyAttackState(
        EnemyStateMachine.EEnemyState key,
        EnemyStateMachine parentStateMachine,
        float attackDelayMin,
        float attackDelayMax,
        float attackRangeMax,
        float attackRangeMin
        ) : base(key)
    {
        _parentStateMachine = parentStateMachine;
        _attackDelayMin = attackDelayMin;
        _attackDelayMax = attackDelayMax;
        _attackRangeMin = attackRangeMin;
        _attackRangeMax = attackRangeMax;
    }

    private EnemyStateMachine _parentStateMachine;

    private float _attackDelayMin;
    private float _attackDelayMax;
    private float _attackDelayCurrent;
    private float _attackRangeMin;
    private float _attackRangeMax;


    public override void EnterState()
    {
        base.EnterState();

        _attackDelayCurrent = Random.Range(_attackDelayMin, _attackDelayMax);
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if (_parentStateMachine.GetCurrentTargetTransform() == null)
            return EnemyStateMachine.EEnemyState.Idle;

        if (Vector2.Distance(_parentStateMachine.GetPosition(), _parentStateMachine.GetCurrentTargetTransform().position) > _attackRangeMax)
            return EnemyStateMachine.EEnemyState.Chase;

        return StateKey;
    }

    public override void UpdateState()
    {
        if (_attackDelayCurrent > 0)
        {
            _attackDelayCurrent -= Time.deltaTime;
        }
        else
        {
            _parentStateMachine.TriggerAttack();
            _attackDelayCurrent = Random.Range(_attackDelayMin, _attackDelayMax);
        }

        if (_parentStateMachine.GetCurrentTargetTransform() == null)
            return;

        if (Vector2.Distance(_parentStateMachine.GetPosition(), _parentStateMachine.GetCurrentTargetTransform().position) < _attackRangeMin)
        {
            _parentStateMachine.SetMovementDirection(_parentStateMachine.GetPosition() - (Vector2)_parentStateMachine.GetCurrentTargetTransform().position);
            _parentStateMachine.SetAimPosition(_parentStateMachine.GetCurrentTargetTransform().position);
        }
        else
        {
            _parentStateMachine.SetMovementDirection(Vector3.zero);
            _parentStateMachine.SetAimPosition(_parentStateMachine.GetCurrentTargetTransform().position);
        }

    }
}
