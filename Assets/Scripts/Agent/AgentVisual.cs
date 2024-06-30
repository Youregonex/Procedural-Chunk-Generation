using UnityEngine;

public class AgentVisual : AgentMonobehaviourComponent
{
    [SerializeField] private AgentInput _agentInput;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _agentInput = transform.root.GetComponent<AgentInput>();
    }

    private void Update()
    {
        ManageSpriteFlip();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    private void ManageSpriteFlip()
    {
        Vector2 aimPosition = _agentInput.GetAimPosition();

        Vector2 aimDirection = (aimPosition - (Vector2)transform.position).normalized;

        if (aimDirection.x < 0)
        {
            _spriteRenderer.flipX = true;
        }
        else if (aimDirection.x > 0)
        {
            _spriteRenderer.flipX = false;
        }
    }
}
