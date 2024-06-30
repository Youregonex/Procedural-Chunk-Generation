using UnityEngine;

public class AgentAnimation : MonoBehaviour, IAgentComponent
{
    private const string MOVE = "MOVE";
    private const string GET_HIT = "GET_HIT";
    private const string DEATH = "DEATH";

    private Animator _agentAnimator;

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

    public void PlayDeathAnimation()
    {
        _agentAnimator.SetTrigger(DEATH);
    }

    public void DisableComponent()
    {
        this.enabled = false;
    }
}
