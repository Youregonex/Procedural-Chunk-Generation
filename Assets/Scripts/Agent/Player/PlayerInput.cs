using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerInput : AgentInput
{
    private PlayerInputActions _playerInputActions;

    public event EventHandler OnInventoryKeyPressed;

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

    public override Vector2 GetMovementVectorNormalized()
    {
        Vector2 movementVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();
        return movementVector.normalized;
    }

    public Vector2 GetMouseScreenPosition() => Mouse.current.position.ReadValue();
    public Vector2 GetMouseScreenPositionNormalized() => GetMouseScreenPosition().normalized;
    public override Vector2 GetAimPosition() => Camera.main.ScreenToWorldPoint(GetMouseScreenPosition());
    public Vector2 GetAimPositionNormalized() => GetAimPosition().normalized;

    private void PlayerInputActions_Inventory_performed(InputAction.CallbackContext obj)
    {
        OnInventoryKeyPressed?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerInputActions_Attack_performed(InputAction.CallbackContext obj)
    {
        Invoke_OnAgentAttackTriggered();
    }

    private void OnDestroy()
    {
        _playerInputActions.Player.Attack.performed -= PlayerInputActions_Attack_performed;
        _playerInputActions.Player.Inventory.performed -= PlayerInputActions_Inventory_performed;
    }
}
