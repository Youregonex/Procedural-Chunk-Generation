using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class AgentHitbox : AgentMonobehaviourComponent, IDamegeable
{
    [Header("Debug Fields")]
    [SerializeField] private AgentCoreBase _agentCore;
    [SerializeField] private AgentHealthSystem _healthSystem;
    [SerializeField] private CapsuleCollider2D _hitboxCollider;

    private void Awake()
    {
        _hitboxCollider = GetComponent<CapsuleCollider2D>();

        _agentCore = transform.root.GetComponent<AgentCoreBase>();
    }

    private void Start()
    {
        _healthSystem = _agentCore.GetAgentComponent<AgentHealthSystem>();
    }

    public bool IsDead() => _healthSystem.IsDead;

    public void TakeDamage(DamageStruct damageStruct) => _healthSystem.TakeDamage(damageStruct);

    public override void DisableComponent()
    {
        _hitboxCollider.enabled = false;
        this.enabled = false;
    }
}
