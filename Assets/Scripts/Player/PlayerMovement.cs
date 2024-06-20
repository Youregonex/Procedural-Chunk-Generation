using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D), typeof(CharacterStats))]
public class PlayerMovement : MonoBehaviour, IPlayer
{
    private CharacterStats _playerStats;
    private Rigidbody2D _rigidBody2D;
    private PlayerInput _playerInput;
    private bool _canMove = true;

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _lastMovementDirection;
    [SerializeField] private PlayerAnimation _playerAnimation;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        _playerAnimation.OnPlayerAttackAnimationStarted += PlayerAnimation_OnPlayerAttackAnimationStarted;
        _playerAnimation.OnPlayerAttackAnimationFinished += PlayerAnimation_OnPlayerAttackAnimationFinished;
    }

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

        _movementDirection = _playerInput.GetMovementVectorNormalized();

        if (_movementDirection != Vector2.zero)
            _lastMovementDirection = _movementDirection;

        StatsEnum moveSpeedStat = StatsEnum.MoveSpeed;

        _rigidBody2D.velocity = new Vector3(_movementDirection.x, _movementDirection.y, 0f) * _playerStats.GetCurrentStatValue(moveSpeedStat);

        _playerAnimation.ManageMoveAnimation(_movementDirection, _lastMovementDirection);
    }

    private void OnDestroy()
    {
        _playerAnimation.OnPlayerAttackAnimationStarted -= PlayerAnimation_OnPlayerAttackAnimationStarted;
        _playerAnimation.OnPlayerAttackAnimationFinished -= PlayerAnimation_OnPlayerAttackAnimationFinished;
    }

    public Vector2 GetCurrentDirection() => _lastMovementDirection;
}
