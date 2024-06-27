using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D), typeof(CharacterStats))]
public class AgentMovement : MonoBehaviour, IPlayer
{
    [SerializeField] private AgentInput _agentInput;

    private CharacterStats _agentStats;
    private Rigidbody2D _rigidBody2D;
    private bool _canMove = true;

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _lastMovementDirection;
    [SerializeField] private AgentAnimation _agentAnimation;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _agentStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        _agentAnimation.OnAgentAttackAnimationStarted += PlayerAnimation_OnPlayerAttackAnimationStarted;
        _agentAnimation.OnAgentAttackAnimationFinished += PlayerAnimation_OnPlayerAttackAnimationFinished;
    }

    public Vector2 GetCurrentDirection() => _lastMovementDirection;

    private void PlayerAnimation_OnPlayerAttackAnimationFinished(object sender, System.EventArgs e)
    {
        _canMove = true;
    }

    private void PlayerAnimation_OnPlayerAttackAnimationStarted(object sender, System.EventArgs e)
    {
        _canMove = false;
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

        StatsEnum moveSpeedStat = StatsEnum.MoveSpeed;

        _rigidBody2D.velocity = new Vector3(_movementDirection.x, _movementDirection.y, 0f) * _agentStats.GetCurrentStatValue(moveSpeedStat);

        _agentAnimation.ManageMoveAnimation(_movementDirection);
    }

    private void OnDestroy()
    {
        _agentAnimation.OnAgentAttackAnimationStarted -= PlayerAnimation_OnPlayerAttackAnimationStarted;
        _agentAnimation.OnAgentAttackAnimationFinished -= PlayerAnimation_OnPlayerAttackAnimationFinished;
    }
}
