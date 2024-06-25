using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour
{
    private PlayerInputActions _playerInputActions;

    public event EventHandler OnInventoryKeyPressed;
    public event EventHandler OnPlayerAttackInput;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();

        _playerInputActions.Player.Enable();
    }

    private void Start()
    {
        _playerInputActions.Player.Attack.performed += PlayerInputActions_Attack_performed;
        _playerInputActions.Player.Inventory.performed += PlayerInputActions_Inventory_performed;
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 movementVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();

        return movementVector.normalized;
    }

    private void PlayerInputActions_Inventory_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInventoryKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerInputActions_Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPlayerAttackInput?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Attack.performed -= PlayerInputActions_Attack_performed;
        _playerInputActions.Player.Inventory.performed -= PlayerInputActions_Inventory_performed;
    }
}
