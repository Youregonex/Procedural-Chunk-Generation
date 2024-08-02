using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(AgentInput), typeof(Rigidbody2D), typeof(AgentStats))]
public class AgentMovement : AgentMonobehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private AgentAnimation _agentAnimation;
    [SerializeField] private EnemyCore _agentCore;
    [SerializeField] private AgentStats _agentStats;
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private bool _canMove = true;

    [field: SerializeField] public Vector2 MovementDirection { get; private set; }
    [field: SerializeField] public Vector2 LastMovementDirection { get; private set; }

    private void Awake()
    {
        _agentCore = GetComponent<EnemyCore>();
    }

    private void Start()
    {
        _rigidBody = _agentCore.GetAgentRigidBody();
        _agentStats = _agentCore.GetAgentComponent<AgentStats>();
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();
        _agentAnimation = _agentCore.GetAgentComponent<AgentAnimation>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnDisable()
    {
        _rigidBody.velocity = Vector2.zero;
    }

    public override void DisableComponent()
    {
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
