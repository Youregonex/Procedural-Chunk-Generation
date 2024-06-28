using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AgentInput), typeof(Rigidbody2D), typeof(CharacterStats))]
public class AgentMovement : MonoBehaviour, IPlayer
{
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private AgentAnimation _agentAnimation;

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
    }

    private void Start()
    {
        _agentAnimation.OnAgentAttackAnimationStarted += AgentAnimation_OnAgentAttackAnimationStarted;
        _agentAnimation.OnAgentAttackAnimationFinished += AgentAnimation_OnAgentAttackAnimationFinished;
    }

    private void OnDestroy()
    {
        _agentAnimation.OnAgentAttackAnimationStarted -= AgentAnimation_OnAgentAttackAnimationStarted;
        _agentAnimation.OnAgentAttackAnimationFinished -= AgentAnimation_OnAgentAttackAnimationFinished;
    }

    public Vector2 GetCurrentDirection() => _lastMovementDirection;

    private void AgentAnimation_OnAgentAttackAnimationFinished(object sender, System.EventArgs e)
    {
        _canMove = true;
    }

    private void AgentAnimation_OnAgentAttackAnimationStarted(object sender, System.EventArgs e)
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
}
