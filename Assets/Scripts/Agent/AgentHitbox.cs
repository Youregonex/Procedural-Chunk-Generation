using UnityEngine;

public class AgentHitbox : MonoBehaviour, IDamageable, IAgentComponent
{
    [SerializeField] private HealthSystem _healthSystem;


    public bool IsDead() => _healthSystem.IsDead();

    public void TakeDamage(DamageStruct damageStruct) => _healthSystem.TakeDamage(damageStruct);

    public void DisableComponent()
    {
        this.enabled = false;
    }
}
