using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : AgentMonobehaviourComponent
{
    public event Action<DamageStruct> OnDamageTaken;
    public event Action<float, float> OnHealthChanged;
    public event Action OnDeath;

    [Header("Config")]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected float _destructionDelay = 0f;
    [SerializeField] protected AgentHitbox _hitbox;

    [Header("Debug Fields")]
    [SerializeField] private bool _isDead = false;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
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

    public IDamageable GetHitbox() => _hitbox;

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

    public bool IsDead() => _isDead;
}
