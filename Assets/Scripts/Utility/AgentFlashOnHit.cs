using UnityEngine;

public class AgentFlashOnHit : FlashOnHit
{
    [Header("Config")]
    [SerializeField] private AgentHealthSystem _agentHealthSystem;

    private void Start()
    {
        _agentHealthSystem.OnDamageTaken += AgentHealthSystem_OnDamageTaken;
    }

    private void OnDestroy()
    {
        _agentHealthSystem.OnDamageTaken -= AgentHealthSystem_OnDamageTaken;
    }

    private void AgentHealthSystem_OnDamageTaken(DamageStruct damageStruct)
    {
        StopAllCoroutines();

        StartCoroutine(FlashSprite());
    }
}
