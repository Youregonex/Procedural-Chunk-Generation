using UnityEngine;

public class EnemyAttackState : BaseState<EnemyStateMachine.EEnemyState>
{
    public EnemyAttackState(EnemyStateMachine.EEnemyState key) : base(key){}

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override EnemyStateMachine.EEnemyState GetNextState()
    {
        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
