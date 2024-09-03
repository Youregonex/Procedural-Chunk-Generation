using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class AgentHitbox : AgentNetworkBehaviourComponent, IDamageable
{
    [Header("Debug Fields")]
    [SerializeField] private AgentHealthSystem _healthSystem;
    [SerializeField] private CapsuleCollider2D _hitboxCollider;

    public override void Initialize()
    {
        GetAgentCore();

        _hitboxCollider = GetComponent<CapsuleCollider2D>();
        _healthSystem = AgentCore.GetAgentComponent<AgentHealthSystem>();
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    public override void OnNetworkDespawn()
    {
        if (_healthSystem != null)
            _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public bool IsDead() => _healthSystem.IsDead;

    public void TakeDamage(DamageStruct damageStruct) => _healthSystem.TakeDamage(damageStruct);

    public override void DisableComponent()
    {
        _hitboxCollider.enabled = false;
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        _hitboxCollider.enabled = true;
        this.enabled = true;
    }

    public EFactions GetFaction() => AgentCore.Faction;

    private void HealthSystem_OnDeath(AgentHealthSystem agentHealthSystem)
    {
        DisableComponent();
    }
}
