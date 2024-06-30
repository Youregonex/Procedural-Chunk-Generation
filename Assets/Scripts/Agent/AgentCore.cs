using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AgentCore : AgentMonobehaviourComponent
{
    [Header("Config")]
    [SerializeField] private EFactions _faction;
    [SerializeField] private List<AgentMonobehaviourComponent> _disableOnDeathComponents;

    [Header("Agent Components")]
    [SerializeField] private HealthSystem _healthSystem;

    [Header("Debug Fields")]
    [SerializeField] private List<AgentMonobehaviourComponent> _agentComponents;

    public EFactions GetAgentFaction() => _faction;

    private void Awake()
    {
        _healthSystem = base.GetComponent<HealthSystem>();

        InitializeComponents();
    }

    private void Start()
    {
        _healthSystem.OnDeath += HealthSystem_OnDeath;
    }

    private void OnDestroy()
    {
        _healthSystem.OnDeath -= HealthSystem_OnDeath;
    }

    public T GetAgentComponent<T>() where T : AgentMonobehaviourComponent
    {
        T awe = _agentComponents.OfType<T>().FirstOrDefault();

        Debug.Log($"Found {awe.GetType()}");

        return _agentComponents.OfType<T>().FirstOrDefault();
    }

    public override void DisableComponent()
    {
        this.enabled = false;
    }

    private void InitializeComponents()
    {
        _agentComponents.Add(_healthSystem);
    }

    private void HealthSystem_OnDeath()
    {
        foreach(AgentMonobehaviourComponent agentComponent in _disableOnDeathComponents)
        {
            agentComponent.DisableComponent();
        }
    }
}
