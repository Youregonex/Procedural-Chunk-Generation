using UnityEngine;

public class EnemyCombatState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyCombatState(
    BaseEnemyBehaviour.EBaseEnemyStates key,
    BaseEnemyBehaviour enemyBehaviour,
    float attackRangeMin,
    float attackRangeMax,
    float aggroRange,
    float chaseRange,
    float combatRange
    ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;

        _attackRangeMin = attackRangeMin;
        _attackRangeMax = attackRangeMax;
        _aggroRange = aggroRange;
        _chaseRange = chaseRange;
        _combatRange = combatRange;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;

    protected float _attackRangeMin;
    protected float _attackRangeMax;
    protected float _aggroRange;
    protected float _chaseRange;
    protected float _combatRange;

    public override void EnterState()
    {
        base.EnterState();
    }

    public override BaseEnemyBehaviour.EBaseEnemyStates GetNextState()
    {
        if (_enemyBehaviour.GetCurrentTargetTransform() == null)
            return BaseEnemyBehaviour.EBaseEnemyStates.Idle;

        if (_enemyBehaviour.GetDistanceToCurrentTarget() <= _attackRangeMax)
            return BaseEnemyBehaviour.EBaseEnemyStates.Attack;

        if (_enemyBehaviour.GetDistanceToCurrentTarget() >= _combatRange)
            return BaseEnemyBehaviour.EBaseEnemyStates.Chase;

        return StateKey;
    }

    public override void UpdateState()
    {
        _enemyBehaviour.SetMovementDirection(_enemyBehaviour.GetCurrentTargetTransform().position - _enemyBehaviour.transform.position);
        _enemyBehaviour.SetAimPosition(_enemyBehaviour.GetCurrentTargetTransform().position);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
