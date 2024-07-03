using UnityEngine;
using System;
using System.Collections;

public class AgentHealthSystem : AgentMonobehaviourComponent
{
    public event Action<DamageStruct> OnDamageTaken;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    [Header("Config")]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;
    [SerializeField] protected float _destructionDelay = 0f;

    [Header("Debug Fields")]
    [SerializeField] protected bool _isDead = false;
    [SerializeField] protected AgentCoreBase _agentCore;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected AgentHitbox _hitbox;

    public bool IsDead => _isDead;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;

        _agentCore = GetComponent<AgentCoreBase>();
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

    public virtual void TakeDamage(DamageStruct damageStruct)
    {
        if (_isDead)
            return;

        _agentAnimation.ManageGetHitAnimation();
        _currentHealth -= damageStruct.damageAmount;

        OnDamageTaken?.Invoke(damageStruct);

        if (_currentHealth <= 0f)
            _currentHealth = 0f;

        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth == 0f)
        {
            OnDeath?.Invoke();

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
