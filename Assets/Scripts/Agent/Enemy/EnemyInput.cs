using UnityEngine;

[RequireComponent(typeof(EnemyStateMachine))]
public class EnemyInput : AgentInput
{
    [Header("Debug Fields")]
    [SerializeField] private EnemyCore _enemyCore;
    [SerializeField] private EnemyStateMachine _enemyStateMachine;



    private void Awake()
    {
        _enemyCore = GetComponent<EnemyCore>();
    }

    private void Start()
    {
        _enemyStateMachine = _enemyCore.GetEnemyStateMachine();

        _enemyStateMachine.OnTargetInAttackRange += EnemyBehaviour_OnTargetInAttackRange;
        _enemyStateMachine = _enemyCore.GetEnemyStateMachine();
    }

    private void OnDestroy()
    {
        _enemyStateMachine.OnTargetInAttackRange -= EnemyBehaviour_OnTargetInAttackRange;
    }

    private void EnemyBehaviour_OnTargetInAttackRange()
    {
        Invoke_OnAgentAttackTriggered();
    }

    public override Vector2 GetAimPosition()
    {
        return _enemyStateMachine.AimPosition;
    }

    public override Vector2 GetMovementVectorNormalized()
    {
        return _enemyStateMachine.MovementDirection.normalized;
    }
}
