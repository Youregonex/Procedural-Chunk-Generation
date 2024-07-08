using UnityEngine;

public class AgentVisual : AgentMonobehaviourComponent
{
    [Header("Debug Fields")]
    [SerializeField] private AgentCoreBase _agentCore;
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _agentCore = transform.root.GetComponent<AgentCoreBase>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();
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
