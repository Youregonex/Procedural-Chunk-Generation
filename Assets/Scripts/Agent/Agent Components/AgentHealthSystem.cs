using UnityEngine;
using System;
using System.Collections;

public class AgentHealthSystem : AgentMonobehaviourComponent
{
    public event Action<DamageStruct> OnDamageTaken;
    public event Action<float, float> OnHealthChanged;
    public event Action<AgentHealthSystem> OnDeath;

    [field: Header("Config")]
    [SerializeField] public float MaxHealth { get; protected set; }
    [SerializeField] public float CurrentHealth { get; protected set; }
    [SerializeField] protected float _destructionDelay = 0f;

    [Header("Debug Fields")]
    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected EnemyCore _agentCore;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected AgentHitbox _hitbox;

    public bool IsDead => _isDead;

    protected virtual void Awake()
    {
        CurrentHealth = MaxHealth;

        _agentCore = GetComponent<EnemyCore>();
    }

    protected virtual void Start()
    {
        _agentAnimation = _agentCore.GetAgentComponent<AgentAnimation>();
        _hitbox = _agentCore.GetAgentComponent<AgentHitbox>();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    public override void EnableComponent()
    {
        this.enabled = true;
    }

    public virtual void TakeDamage(DamageStruct damageStruct)
    {
        if (_isDead)
            return;

        _agentAnimation.ManageGetHitAnimation();
        CurrentHealth -= damageStruct.damageAmount;

        OnDamageTaken?.Invoke(damageStruct);

        if (CurrentHealth <= 0f)
            CurrentHealth = 0f;

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth == 0f)
        {
            OnDeath?.Invoke(this);
            Die();
        }
    }

    public IDamegeable GetHitbox() => _hitbox;

    protected virtual void Die()
    {
        _isDead = true;
        _hitbox.enabled = false;

        _agentAnimation.PlayDeathAnimation();
        StartCoroutine(DestroyWithDelay());
    }

    protected IEnumerator DestroyWithDelay()
    {
        yield return new WaitForSeconds(_destructionDelay);

        Destroy(gameObject);
    }
}