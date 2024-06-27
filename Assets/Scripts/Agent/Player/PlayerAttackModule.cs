using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerAttackModule : AgentAttackModule
{
    protected override void Attack()
    {
        if (_attackCooldownCurrent > 0 || IsPointerOverUIObject() || !_canAttack)
            return;

        _attackCooldownCurrent = _attackCooldownMax;

        _currentWeapon.Attack();
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
