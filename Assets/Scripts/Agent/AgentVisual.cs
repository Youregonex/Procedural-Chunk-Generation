using UnityEngine;

public class AgentVisual : MonoBehaviour, IAgentComponent
{
    [SerializeField] private AgentMovement _agentMovement;
    [SerializeField] private AgentInput _agentInput;
    [SerializeField] private HealthSystem _healthSystem;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _agentMovement = transform.root.GetComponent<AgentMovement>();
        _agentInput = transform.root.GetComponent<AgentInput>();
        _healthSystem = transform.root.GetComponent<HealthSystem>();
    }

    private void Start()
    {
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void Update()
    {
        ManageWeaponSpriteFlip();
    }

    private void OnDestroy()
    {
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public void DisableComponent()
    {
        this.enabled = false;
    }

    private void HealthSystem_OnDeath()
    {
        this.enabled = false;
    }

    private void ManageWeaponlessSpriteFlip()
    {
        if (_agentMovement.GetCurrentDirection().x != 0 && _agentMovement.GetCurrentDirection().x > 0)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private void ManageWeaponSpriteFlip()
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
