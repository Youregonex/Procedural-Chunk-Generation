using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AgentInput), typeof(Rigidbody2D), typeof(CharacterStats))]
public class AgentMovement : AgentMonobehaviourComponent
{
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private AgentAnimation _agentAnimation;

    private CharacterStats _agentStats;
    private Rigidbody2D _rigidBody2D;
    private bool _canMove = true;

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _lastMovementDirection;
    [SerializeField] private CapsuleCollider2D _collisionCollider;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _agentStats = GetComponent<CharacterStats>();
        _collisionCollider = GetComponent<CapsuleCollider2D>();
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
