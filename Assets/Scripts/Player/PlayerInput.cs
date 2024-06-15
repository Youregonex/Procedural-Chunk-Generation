using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Enable();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 movementVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();

        return movementVector.normalized;
    }

}
