using UnityEngine;

public class EnemyCombatState : BaseState<BaseEnemyBehaviour.EBaseEnemyStates>
{
    public EnemyCombatState(
    BaseEnemyBehaviour.EBaseEnemyStates key,
    BaseEnemyBehaviour enemyBehaviour,
    BaseEnemyCombatStateDataSO baseEnemyCombatStateDataSO
    ) : base(key)
    {
        _enemyBehaviour = enemyBehaviour;
        _baseEnemyCombatStateDataSO = baseEnemyCombatStateDataSO;
    }

    protected BaseEnemyBehaviour _enemyBehaviour;
    protected BaseEnemyCombatStateDataSO _baseEnemyCombatStateDataSO;


}
