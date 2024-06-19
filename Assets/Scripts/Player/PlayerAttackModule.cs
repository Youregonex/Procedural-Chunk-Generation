using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttackModule : MonoBehaviour
{
    [SerializeField] private float _attackCooldownCurrent;
    [SerializeField] private float _attackCooldownMax = 1f;
    [SerializeField] private float _attackRadius;
    [SerializeField] private PlayerAnimation _playerAnimation;

    private PlayerMovement _playerMovement;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        _playerInput.OnPlayerAttackInput += PlayerInput_OnPlayerAttackInput;
    }

    private void Update()
    {
        if (_attackCooldownCurrent > 0)
            _attackCooldownCurrent -= Time.deltaTime;
    }

    private void PlayerInput_OnPlayerAttackInput(object sender, System.EventArgs e)
    {
        Attack();
    }

    private void Attack()
    {
        if (_attackCooldownCurrent > 0)
            return;

        _attackCooldownCurrent = _attackCooldownMax;

        Collider2D[] targets = Physics2D.OverlapCircleAll(_playerMovement.GetCurrentDirection(), _attackRadius);

        foreach (Collider2D target in targets)
        {

        }

        _playerAnimation.ManageAttackAnimation();
    }

    private void OnDestroy()
    {
        _playerInput.OnPlayerAttackInput -= PlayerInput_OnPlayerAttackInput;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new(EventSystem.current);
        eventDataCurrentPosition.position = Mouse.current.position.ReadValue();
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }
}
