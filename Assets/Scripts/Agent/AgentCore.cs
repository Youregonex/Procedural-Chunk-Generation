using UnityEngine;
using System.Collections.Generic;

public class AgentCore : MonoBehaviour, IAgentComponent
{
    [Header("Config")]
    [SerializeField] private EFactions _faction;
    [SerializeField] private List<IAgentComponent> _disableOnDeathComponents;

    [Header("Agent Components")]
    [SerializeField] private HealthSystem _healthSystem;

    public EFactions GetAgentFaction() => _faction;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    public void DisableComponent()
    {
        this.enabled = false;
    }

    private void HealthSystem_OnDeath()
    {
        foreach(IAgentComponent agentComponent in _disableOnDeathComponents)
        {
            agentComponent.DisableComponent();
        }
    }
}
