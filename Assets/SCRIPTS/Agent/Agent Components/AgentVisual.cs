using UnityEngine;

public class AgentVisual : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] private GameObject _shadowGameObject;

    [Header("Debug Fields")]
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private AgentCoreBase _agentCore;
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _agentCore = transform.root.GetComponent<AgentCoreBase>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();

        if(_shadowGameObject != null)
            _shadowGameObject.SetActive(false);
    }

    private void Start()
    {
        _agentInput = _agentCore.GetAgentComponent<AgentInput>();
    }

    private void Update()
    {
        ManageSpriteFlip();
    }

    public void EnableShadow()
    {
        if (_shadowGameObject != null)
            _shadowGameObject.SetActive(true);
    }
    public void DisableShadow()
    {
        if (_shadowGameObject != null)
            _shadowGameObject.SetActive(false);
    }

    public GameObject GetShadowGameObject() => _shadowGameObject;

    public void DisableTrailRenderer()
    {
        if(_trailRenderer != null)
            _trailRenderer.emitting = false;
    }

    public void EnableTrailRenderer()
    {
        if (_trailRenderer != null)
            _trailRenderer.emitting = true;
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
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
