using UnityEngine;

public class AgentStatHealthSystem : AgentHealthSystem
{
    [Header("Config")]
    [SerializeField] private float _vitalityToHealthRatio;
    [SerializeField] private float _initialHealth;

    [Header("Debug Fields")]
    [SerializeField] private AgentStats _agentStats;

    protected override void Awake()
    {
        _agentCore = GetComponent<AgentCoreBase>();
        _agentStats = _agentCore.GetAgentComponent<AgentStats>();

        float vitalityValue = _agentStats.GetCurrentStatValue(EStats.Vitality);
        MaxHealth = _initialHealth + (vitalityValue * _vitalityToHealthRatio);
        CurrentHealth = MaxHealth;
    }
}
