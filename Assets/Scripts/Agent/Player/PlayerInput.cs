using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class PlayerInput : AgentInput
{
    private PlayerInputActions _playerInputActions;

    public event Action OnInventoryKeyPressed;
    public event Action OnInteractKeyPressed;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
    }

    private void Start()
    {
        _playerInputActions.Player.Attack.performed += PlayerInputActions_Attack_performed;
        _playerInputActions.Player.Inventory.performed += PlayerInputActions_Inventory_performed;
        _playerInputActions.Player.Interact.performed += PlayerInputActions_Interact_performed;
    }

    private void PlayerInputActions_Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractKeyPressed?.Invoke();
    }

    public override Vector2 GetMovementVectorNormalized()
    {
        Vector2 movementVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();
        return movementVector.normalized;
    }

    public Vector2 GetMouseScreenPosition() => Mouse.current.position.ReadValue();
    public Vector2 GetMouseScreenPositionNormalized() => GetMouseScreenPosition().normalized;
    public override Vector2 GetAimPosition() => Camera.main.ScreenToWorldPoint(GetMouseScreenPosition());

    private void PlayerInputActions_Inventory_performed(InputAction.CallbackContext obj)
    {
        OnInventoryKeyPressed?.Invoke();
    }

    private void PlayerInputActions_Attack_performed(InputAction.CallbackContext obj)
    {
        Invoke_OnAgentAttackTriggered();
    }

    private void OnDestroy()
    {
        if (_playerInputActions == null)
            return;

        _playerInputActions.Player.Attack.performed -= PlayerInputActions_Attack_performed;
        _playerInputActions.Player.Inventory.performed -= PlayerInputActions_Inventory_performed;
        _playerInputActions.Player.Interact.performed -= PlayerInputActions_Interact_performed;
    }
}
