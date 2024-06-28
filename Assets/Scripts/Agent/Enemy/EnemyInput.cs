using UnityEngine;

[RequireComponent(typeof(EnemyBehaviour))]
public class EnemyInput : AgentInput
{
    [Header("Debug Fields")]
    [SerializeField] private EnemyBehaviour _enemyBehaviour;

    private void Awake()
    {
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
    }

    private void Start()
    {
        _enemyBehaviour.OnTargetInAttackRange += EnemyBehaviour_OnTargetInAttackRange;
    }

    private void OnDestroy()
    {
        _enemyBehaviour.OnTargetInAttackRange -= EnemyBehaviour_OnTargetInAttackRange;
    }

    private void EnemyBehaviour_OnTargetInAttackRange(object sender, System.EventArgs e)
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
