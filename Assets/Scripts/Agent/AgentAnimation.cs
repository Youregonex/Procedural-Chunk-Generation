using UnityEngine;
using System;

public class AgentAnimation : MonoBehaviour
{
    private const string MOVE = "MOVE";
    private const string GET_HIT = "GET_HIT";

    private Animator _agentAnimator;

    public event EventHandler OnAgentAttackAnimationStarted;
    public event EventHandler OnAgentAttackAnimationFinished;

    private void Start()
    {
        _agentAnimator = GetComponent<Animator>();
    }

    public void ManageMoveAnimation(Vector2 movementDirection)
    {
        _agentAnimator.SetBool(MOVE, movementDirection.x != 0 || movementDirection.y != 0);
    }

    public void ManageGetHitAnimation()
    {
        _agentAnimator.SetTrigger(GET_HIT);
    }

    private void AgentAttackAnimationStarted() // Used by Animation Event
    {
        OnAgentAttackAnimationStarted?.Invoke(this, EventArgs.Empty);
    }

    private void AgentAttackAnimationFinished() // Used by Animation Event
    {
        OnAgentAttackAnimationFinished?.Invoke(this, EventArgs.Empty);
    }
}
