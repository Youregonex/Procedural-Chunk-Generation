using UnityEngine;
using System;
using System.Collections;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public event Action<DamageStruct> OnDamageTaken;
    public event Action OnDeath;

    [Header("Config")]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;
    [SerializeField] protected AgentAnimation _agentAnimation;
    [SerializeField] protected float _destructionDelay = 0f;

    [Header("Debug Fields")]
    [SerializeField] private CapsuleCollider2D _hitCollider;
    [SerializeField] private bool _isDead = false;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
        _hitCollider = GetComponent<CapsuleCollider2D>();
    }

    public virtual void TakeDamage(DamageStruct damageStruct)
    {
        if (_isDead)
            return;

        _agentAnimation.ManageGetHitAnimation();
        _currentHealth -= damageStruct.damageAmount;

        OnDamageTaken?.Invoke(damageStruct);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;

            OnDeath?.Invoke();

            Die();
        }
    }

    protected virtual void Die()
    {
        _isDead = true;
        _hitCollider.enabled = false;

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
