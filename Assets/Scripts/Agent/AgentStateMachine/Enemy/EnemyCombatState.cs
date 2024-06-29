using UnityEngine;

public class EnemyCombatState : BaseState<EnemyStateMachine.EEnemyState>
{
    public EnemyCombatState(
    EnemyStateMachine.EEnemyState key,
    EnemyStateMachine parentStateMachine
    ) : base(key)
    {
        _parentStateMachine = parentStateMachine;
    }

    private EnemyStateMachine _parentStateMachine;

    public override void UpdateState()
    {

    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        return StateKey;
    }
}
