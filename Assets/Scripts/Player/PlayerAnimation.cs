using UnityEngine;
using System;

public class PlayerAnimation : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    private const string CURRENT_DIRECTION_X = "CurrentDirX";
    private const string CURRENT_DIRECTION_Y = "CurrentDirY";
    private const string IS_MOVING = "IsMoving";
    private const string ATTACK = "Attack";

    private Animator _playerAnimator;

    public event EventHandler OnPlayerAttackAnimationStarted;
    public event EventHandler OnPlayerAttackAnimationFinished;

    private void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    public void ManageMoveAnimation(Vector2 movementDirection, Vector2 lastMove)
    {
        _playerAnimator.SetFloat(HORIZONTAL, movementDirection.x);
        _playerAnimator.SetFloat(VERTICAL, movementDirection.y);

        _playerAnimator.SetFloat(CURRENT_DIRECTION_X, lastMove.x);
        _playerAnimator.SetFloat(CURRENT_DIRECTION_Y, lastMove.y);

        _playerAnimator.SetBool(IS_MOVING, movementDirection.x != 0 || movementDirection.y != 0);
    }

    public void ManageAttackAnimation()
    {
        _playerAnimator.SetTrigger(ATTACK);
    }

    private void PlayerAttackAnimationStarted() // Used by Animation Event
    {
        OnPlayerAttackAnimationStarted?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerAttackAnimationFinished() // Used by Animation Event
    {
        OnPlayerAttackAnimationFinished?.Invoke(this, EventArgs.Empty);
    }
}
