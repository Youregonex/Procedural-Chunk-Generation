using UnityEngine;

public class EnemyIdleState : BaseState<EnemyStateMachine.EEnemyState>
{
    public EnemyIdleState(
        EnemyStateMachine.EEnemyState key,
        EnemyStateMachine parentStateMachine,
        float timeToStartRoamMax,
        Vector2 roamPositionOffsetMax
        ) : base(key)
    {
        _parentStateMachine = parentStateMachine;
        _timeToStartRoamMax = timeToStartRoamMax;
        _roamPositionOffsetMax = roamPositionOffsetMax;
    }

    private EnemyStateMachine _parentStateMachine;

    private float _timeToStartRoamMax;
    private Vector2 _roamPositionOffsetMax;

    private Vector2 _currentRoamPosition = Vector2.zero;


    public override void EnterState()
    {
        base.EnterState();

        _currentRoamPosition = Vector2.zero;
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        if(_parentStateMachine.GetTargetTransformList().Count != 0)
            return EnemyStateMachine.EEnemyState.Chase;


        if (_currentRoamPosition != Vector2.zero)
            return EnemyStateMachine.EEnemyState.Roam;

        return StateKey;
    }

    public override void UpdateState()
    {
        _parentStateMachine.SetMovementDirection(Vector2.zero);

        if(StateCurrentTime >= _timeToStartRoamMax)
            _currentRoamPosition = PickRandomRoamPosition();
    }

    public override void ExitState()
    {
        _parentStateMachine.SetCurrentRoamPosition(_currentRoamPosition);
    }

    private Vector2 PickRandomRoamPosition()
    {
        float roamPositionX = Random.Range(-_roamPositionOffsetMax.x, _roamPositionOffsetMax.x);
        float roamPositionY = Random.Range(-_roamPositionOffsetMax.y, _roamPositionOffsetMax.y);

        Vector2 randomRoamPosition = new Vector2(roamPositionX, roamPositionY);

        return _parentStateMachine.GetPosition() + randomRoamPosition;
    }
}
