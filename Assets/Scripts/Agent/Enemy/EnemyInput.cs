using UnityEngine;

[RequireComponent(typeof(EnemyStateMachine))]
public class EnemyInput : AgentInput
{
    [Header("Debug Fields")]
    [SerializeField] private EnemyStateMachine _enemyBehaviour;

    private void Awake()
    {
        _enemyBehaviour = GetComponent<EnemyStateMachine>();
    }

    private void Start()
    {
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
        return _enemyBehaviour.AimPosition;
    }

    public override Vector2 GetMovementVectorNormalized()
    {
        return _enemyBehaviour.MovementDirection.normalized;
    }
}