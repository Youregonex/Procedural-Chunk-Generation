using UnityEngine;

public class EnemyChaseState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyChaseState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        float aggroRange,
        float chaseRange,
        float combatRange
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;
        _aggroRange = aggroRange;
        _chaseRange = chaseRange;
        _combatRange = combatRange;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;

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

        if (_enemyBehaviour.GetDistanceToCurrentTarget() <= _combatRange)
            return BaseEnemyBehaviour.EBaseEnemyStates.Combat;

        if (_enemyBehaviour.GetDistanceToCurrentTarget() > _chaseRange)
            return BaseEnemyBehaviour.EBaseEnemyStates.Idle;

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
