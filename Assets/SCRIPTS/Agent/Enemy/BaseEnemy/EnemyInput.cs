using UnityEngine;

public class EnemyInput : AgentInput
{
    [Header("Debug Fields")]
    [SerializeField] private BaseEnemyBehaviour _enemyBehaviour;

    public EnemyCore EnemyCore => AgentCore as EnemyCore;

    public override void Initialize()
    {
        GetAgentCore();

        _enemyBehaviour = EnemyCore.GetAgentComponent<BaseEnemyBehaviour>();
        _enemyBehaviour.OnTargetInAttackRange += EnemyBehaviour_OnTargetInAttackRange;
    }

    public override void OnNetworkDespawn()
    {
        _enemyBehaviour.OnTargetInAttackRange -= EnemyBehaviour_OnTargetInAttackRange;
    }

    private void EnemyBehaviour_OnTargetInAttackRange()
    {
        Invoke_AgentInput_OnMousePrimary();
    }

    public override Vector2 GetAimPosition()
    {
        if (_enemyBehaviour == null)
            return Vector2.zero;

        return _enemyBehaviour.AimPosition;
    }

    public override Vector2 GetMovementVectorNormalized()
    {
        return _enemyBehaviour.MovementDirection.normalized;
    }
}
