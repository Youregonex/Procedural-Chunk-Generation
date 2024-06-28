using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour, IDamageable
{
    public event Action<DamageStruct> OnDamageTaken;

    [Header("Debug Fields")]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;

    public FactionEnum Faction { get; set; }

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(DamageStruct damageStruct)
    {
        _currentHealth -= damageStruct.damageAmount;

        OnDamageTaken?.Invoke(damageStruct);

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
