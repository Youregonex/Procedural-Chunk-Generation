using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class Enemy : MonoBehaviour, IDamageable
{
    private HealthSystem _enemyHealthSystem;

    public FactionEnum Faction { get; set; }


    private void Awake()
    {
        _enemyHealthSystem = GetComponent<HealthSystem>();
    }

    public void TakeDamage(DamageStruct damageStruct)
    {
        _enemyHealthSystem.TakeDamage(damageStruct);
    }
}
