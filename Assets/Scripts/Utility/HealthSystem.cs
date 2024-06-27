using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("Debug Fields")]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _currentHealth;

    protected virtual void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public virtual void TakeDamage(DamageStruct damageStruct)
    {
        _currentHealth -= damageStruct.damageAmount;

        if(_currentHealth <= 0)
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
