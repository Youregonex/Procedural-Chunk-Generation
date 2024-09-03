using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AgentInput), typeof(Rigidbody2D), typeof(AgentStats))]
public class AgentMovement : AgentNetworkBehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private AgentAnimation _agentAnimation;
    [SerializeField] private AgentStats _agentStats;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private bool _canMove = true;

    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 LastMovementDirection { get; private set; }

    public override void Initialize()
    {
        GetAgentCore();
        _rigidBody = AgentCore.AgentRigidBody;

        _agentStats = AgentCore.GetAgentComponent<AgentStats>();
        _agentInput = AgentCore.GetAgentComponent<AgentInput>();
        _agentAnimation = AgentCore.GetAgentComponent<AgentAnimation>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    public override void DisableComponent()
    {
        if(_rigidBody != null)
            _rigidBody.velocity = Vector2.zero;

        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    private void HandleMovement()
    {
        if (!_canMove)
        {
            _rigidBody.velocity = Vector2.zero;

            return;
        }

        MovementDirection = _agentInput.GetMovementVectorNormalized();

        if (MovementDirection != Vector2.zero)
            LastMovementDirection = MovementDirection;

        EStats moveSpeedStat = EStats.MoveSpeed;

        _rigidBody.velocity = new Vector3(MovementDirection.x, MovementDirection.y, 0f) * _agentStats.GetCurrentStatValue(moveSpeedStat);
        _agentAnimation.ManageMoveAnimation(MovementDirection);
    }
}
