using UnityEngine;

public class EnemyRoamState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyRoamState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        BaseEnemyRoamStateDataSO baseEnemyRoamStateDataSO
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;
        _baseEnemyRoamStateDataSO = baseEnemyRoamStateDataSO;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;
    protected BaseEnemyRoamStateDataSO _baseEnemyRoamStateDataSO;
  
}
