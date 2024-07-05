using UnityEngine;

public class EnemyIdleState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyIdleState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        float timeToStartRoamMin,
        float timeToStartRoamMax,
        Vector2 roamPositionOffsetMax
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;

        _timeToStartRoamMin = timeToStartRoamMin;
        _timeToStartRoamMax = timeToStartRoamMax;
        _roamPositionOffsetMax = roamPositionOffsetMax;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;

    protected float _timeToStartRoamMin;
    protected float _timeToStartRoamMax;
    protected Vector2 _roamPositionOffsetMax;

    private float _timeToStartRoam;
    private Vector2 _roamPosition;

    public override void EnterState()
    {
        base.EnterState();

        _timeToStartRoam = Random.Range(_timeToStartRoamMin, _timeToStartRoamMax);

        _roamPosition = (Vector2)_enemyBehaviour.transform.position +
                        new Vector2(
                                    Random.Range(-_roamPositionOffsetMax.x, _roamPositionOffsetMax.x),
                                    Random.Range(-_roamPositionOffsetMax.y, _roamPositionOffsetMax.y));
    }

    public override BaseEnemyBehaviour.EBaseEnemyStates GetNextState()
    {
        if (_enemyBehaviour.GetCurrentTargetTransform() != null)
            return BaseEnemyBehaviour.EBaseEnemyStates.Combat;

        if (StateCurrentTime >= _timeToStartRoam)
            return BaseEnemyBehaviour.EBaseEnemyStates.Roam;

        return StateKey;
    }

    public override void UpdateState()
    {
        _enemyBehaviour.SetMovementDirection(Vector2.zero);

    }

    public override void ExitState()
    {
        _enemyBehaviour.SetRoamPosition(_roamPosition);
    }
}
