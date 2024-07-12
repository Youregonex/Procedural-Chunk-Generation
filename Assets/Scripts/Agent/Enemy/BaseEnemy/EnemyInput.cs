using UnityEngine;

public class EnemyInput : AgentInput
{
    [Header("Debug Fields")]
    [SerializeField] private EnemyCore _enemyCore;
    [SerializeField] private BaseEnemyBehaviour _enemyBehaviour;


    private void Awake()
    {
        _enemyCore = GetComponent<EnemyCore>();
    }

    private void Start()
    {
        _enemyBehaviour = _enemyCore.GetAgentComponent<BaseEnemyBehaviour>();

        _enemyBehaviour.OnTargetInAttackRange += EnemyBehaviour_OnTargetInAttackRange;
    }

    private void OnDestroy()
    {
        _enemyBehaviour.OnTargetInAttackRange -= EnemyBehaviour_OnTargetInAttackRange;
    }

    private void EnemyBehaviour_OnTargetInAttackRange()
    {
        Invoke_OnAgentAttackTriggered();
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
