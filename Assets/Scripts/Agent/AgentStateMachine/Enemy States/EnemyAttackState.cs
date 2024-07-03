using UnityEngine;

public class EnemyAttackState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyAttackState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        BaseEnemyAttackStateDataSO baseEnemyAttackStateDataSO
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;
        _attackStateDataSO = baseEnemyAttackStateDataSO;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;
    protected BaseEnemyAttackStateDataSO _attackStateDataSO;


   

    
}
