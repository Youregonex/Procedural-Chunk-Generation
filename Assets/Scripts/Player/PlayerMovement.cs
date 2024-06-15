using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D), typeof(CharacterStats))]
public class PlayerMovement : MonoBehaviour, IPlayer
{
    private CharacterStats _playerStats;
    private Rigidbody2D _rigidBody2D;
    private PlayerInput _playerInput;

    [Header("Debug Fields")]
    [SerializeField] private Vector2 _movementDirection;
    [SerializeField] private Vector2 _lastMovementDirection;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _playerStats = GetComponent<CharacterStats>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        _movementDirection = _playerInput.GetMovementVectorNormalized();

        if (_movementDirection != Vector2.zero)
            _lastMovementDirection = _movementDirection;

        StatsEnum moveSpeedStat = StatsEnum.MoveSpeed;

        _rigidBody2D.velocity = new Vector3(_movementDirection.x, _movementDirection.y, 0f) * _playerStats.GetCurrentStatValue(moveSpeedStat);
    }
}
