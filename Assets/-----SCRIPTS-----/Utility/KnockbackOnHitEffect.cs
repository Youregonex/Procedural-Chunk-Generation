using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AgentHealthSystem), typeof(Rigidbody2D), typeof(AgentMovement))]
public class KnockbackOnHitEffect : MonoBehaviour
{
    [SerializeField] private float _knockbackDuration;

    private Rigidbody2D _rigidBody2D;
    private AgentHealthSystem _healthSystem;
    private AgentMovement _agentMovement;

    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _healthSystem = GetComponent<AgentHealthSystem>();
        _agentMovement = GetComponent<AgentMovement>();
    }

    private void Start()
    {
        _healthSystem.OnDamageTaken += HealthSystem_OnDamageTaken;
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void OnDestroy()
    {
        _healthSystem.OnDamageTaken -= HealthSystem_OnDamageTaken;
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    private void HealthSystem_OnDeath(AgentHealthSystem agentHealthSystem)
    {
        StopKnockBack();
    }

    private IEnumerator ApplyKnockback(DamageStruct damageStruct)
    {
        _agentMovement.enabled = false;

        Vector2 knockbackDirection = (transform.position - damageStruct.damageSender.transform.position).normalized;
        _rigidBody2D.AddForce(knockbackDirection* damageStruct.knockbackForce, ForceMode2D.Impulse);


        yield return new WaitForSeconds(_knockbackDuration);

        _rigidBody2D.velocity = Vector2.zero;
        _agentMovement.enabled = true;
    }

    private void StopKnockBack()
    {
        StopAllCoroutines();

        _rigidBody2D.velocity = Vector2.zero;
    }

    private void HealthSystem_OnDamageTaken(DamageStruct damageStruct)
    {
        StopAllCoroutines();

        StartCoroutine(ApplyKnockback(damageStruct));
    }

}
