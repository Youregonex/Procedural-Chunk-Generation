using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AgentInput), typeof(Rigidbody2D), typeof(AgentStats))]
public class AgentMovement : AgentMonobehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private AgentAnimation _agentAnimation;
    [SerializeField] private AgentCoreBase _agentCore;
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _lastMovementDirection;
    [SerializeField] private CapsuleCollider2D _collisionCollider;
    [SerializeField] private AgentStats _agentStats;
    [SerializeField] private Rigidbody2D _rigidBody2D;
    [SerializeField] private bool _canMove = true;

    private void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
    }

    private void Start()
    {
        _rigidBody2D = _agentCore.GetAgentRigidBody2D();
        _agentStats = _agentCore.GetAgentStats();
        _collisionCollider = _agentCore.GetAgentCollider();

        _agentInput = _agentCore.GetAgentComponent<AgentInput>();
        _agentAnimation = _agentCore.GetAgentComponent<AgentAnimation>();
    }

    public override void DisableComponent()
    {
        _rigidBody2D.velocity = Vector2.zero;
        _collisionCollider.enabled = false;
        this.enabled = false;
    }

    public Vector2 GetCurrentDirection() => _lastMovementDirection;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!_canMove)
        {
            _rigidBody2D.velocity = Vector2.zero;

            return;
        }

        _movementDirection = _agentInput.GetMovementVectorNormalized();

        if (_movementDirection != Vector2.zero)
            _lastMovementDirection = _movementDirection;

        EStats moveSpeedStat = EStats.MoveSpeed;

        _rigidBody2D.velocity = new Vector3(_movementDirection.x, _movementDirection.y, 0f) * _agentStats.GetCurrentStatValue(moveSpeedStat);

        _agentAnimation.ManageMoveAnimation(_movementDirection);
    }
}
