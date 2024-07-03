using UnityEngine;

public class EnemyIdleState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyIdleState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        BaseEnemyIdleStateDataSO idleStateDataSO
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;
        _idleStateDataSO = idleStateDataSO;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;
    protected BaseEnemyIdleStateDataSO _idleStateDataSO;




}
