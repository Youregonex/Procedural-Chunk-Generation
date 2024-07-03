using UnityEngine;

public class EnemyChaseState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyChaseState(
        BaseEnemyBehaviour.EBaseEnemyStates key,
        BaseEnemyBehaviour enemyBehaviour,
        BaseEnemyChaseStateDataSO baseEnemyChaseStateDataSO
        ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;
        _baseEnemyChaseStateDataSO = baseEnemyChaseStateDataSO;
    }

    private BaseEnemyBehaviour _enemyBehaviour;
    private BaseEnemyChaseStateDataSO _baseEnemyChaseStateDataSO;



}
