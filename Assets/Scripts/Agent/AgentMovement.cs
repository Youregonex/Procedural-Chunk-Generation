using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AgentInput), typeof(Rigidbody2D), typeof(CharacterStats))]
public class AgentMovement : MonoBehaviour, IAgentComponent
{
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private AgentAnimation _agentAnimation;
    [SerializeField] private HealthSystem _healthSystem;

    private CharacterStats _agentStats;
    private Rigidbody2D _rigidBody2D;
    private bool _canMove = true;

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _lastMovementDirection;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _agentStats = GetComponent<CharacterStats>();
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void OnDestroy()
    {
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public void DisableComponent()
    {
        this.enabled = false;
    }

    public Vector2 GetCurrentDirection() => _lastMovementDirection;

    private void HealthSystem_OnDeath()
    {
        _rigidBody2D.velocity = Vector2.zero;
        this.enabled = false;
    }

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
