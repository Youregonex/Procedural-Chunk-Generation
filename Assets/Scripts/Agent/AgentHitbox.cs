using UnityEngine;

public class AgentHitbox : AgentMonobehaviourComponent, IDamageable
{
    [Header("Debug Fields")]
    [SerializeField] private HealthSystem _healthSystem;
    [SerializeField] private CapsuleCollider2D _hitboxCollider;

    private void Awake()
    {
        _hitboxCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        _healthSystem = transform.root.GetComponent<AgentCore>().GetAgentComponent<HealthSystem>();
    }

    public bool IsDead() => _healthSystem.IsDead();

    public void TakeDamage(DamageStruct damageStruct) => _healthSystem.TakeDamage(damageStruct);

    public override void DisableComponent()
    {
        _hitboxCollider.enabled = false;
        this.enabled = false;
    }
}
