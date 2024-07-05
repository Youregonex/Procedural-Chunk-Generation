using UnityEngine;

public class EnemyRoamState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyRoamState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        float roamTimeMax
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;

        _roamTimeMax = roamTimeMax;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;

    protected float _roamTimeMax;

    private float _roamDistanceThreshold = .2f;

    public override void EnterState()
    {
        base.EnterState();
    }

    public override BaseEnemyBehaviour.EBaseEnemyStates GetNextState()
    {
        if (_enemyBehaviour.GetCurrentTargetTransform() != null)
            return BaseEnemyBehaviour.EBaseEnemyStates.Combat;

        if (StateCurrentTime > _roamTimeMax ||
            _enemyBehaviour.CurrentRoamPosition == Vector2.zero ||
            Vector2.Distance(
                _enemyBehaviour.CurrentRoamPosition,
                _enemyBehaviour.transform.position) <= _roamDistanceThreshold)

            return BaseEnemyBehaviour.EBaseEnemyStates.Idle;


        return StateKey;
    }

    public override void UpdateState()
    {
        _enemyBehaviour.SetMovementDirection(_enemyBehaviour.CurrentRoamPosition - (Vector2)_enemyBehaviour.transform.position);
        _enemyBehaviour.SetAimPosition(_enemyBehaviour.CurrentRoamPosition);
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
