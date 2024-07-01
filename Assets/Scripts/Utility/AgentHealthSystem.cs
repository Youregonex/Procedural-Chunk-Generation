using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class AgentHealthSystem : HealthSystem
{
    [Header("Config")]
    [SerializeField] private float _vitalityToHealthRatio;
    [SerializeField] private float _initialHealth;

    [Header("Debug Fields")]
    [SerializeField] private AgentStats _agentStats;

    protected override void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
    }

    protected override void Start()
    {
        base.Start();

        _agentStats = _agentCore.GetAgentComponent<AgentStats>();

        float vitalityValue = _agentStats.GetCurrentStatValue(EStats.Vitality);
        _maxHealth = _initialHealth + (vitalityValue * _vitalityToHealthRatio);
        _currentHealth = _maxHealth;
    }
}
