using UnityEngine;

public class EnemyAttackState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyAttackState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        float attackRangeMin,
        float attackRangeMax,
        float attackDelayMin,
        float attackDelayMax
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;

        _attackRangeMin = attackRangeMin;
        _attackRangeMax = attackRangeMax;
        _attackDelayMin = attackDelayMin;
        _attackDelayMax = attackDelayMax;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;

    protected float _attackRangeMin;
    protected float _attackRangeMax;
    protected float _attackDelayMin;
    protected float _attackDelayMax;

    private float _attackDelayCurrent;

    public override void EnterState()
    {
        base.EnterState();

        _attackDelayCurrent = Random.Range(_attackDelayMin, _attackDelayMax);
    }

    public override BaseEnemyBehaviour.EBaseEnemyStates GetNextState()
    {
        if (_enemyBehaviour.GetCurrentTargetTransform() == null)
            return BaseEnemyBehaviour.EBaseEnemyStates.Idle;

        if (_enemyBehaviour.GetDistanceToCurrentTarget() > _attackRangeMax)
            return BaseEnemyBehaviour.EBaseEnemyStates.Combat;


        return StateKey;
    }

    public override void UpdateState()
    {
        _attackDelayCurrent -= Time.deltaTime;

        if (_enemyBehaviour.GetAttackCooldown() <= 0 && _attackDelayCurrent <= 0)
        {
            _enemyBehaviour.TriggerAttack();
            _attackDelayCurrent = Random.Range(_attackDelayMin, _attackDelayMax);
        }

        _enemyBehaviour.SetAimPosition(_enemyBehaviour.GetCurrentTargetTransform().position);

        if (_enemyBehaviour.GetDistanceToCurrentTarget() < _attackRangeMin)
            _enemyBehaviour.SetMovementDirection(_enemyBehaviour.transform.position - _enemyBehaviour.GetCurrentTargetTransform().position);
        else
            _enemyBehaviour.SetMovementDirection(Vector2.zero);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
