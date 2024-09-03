using UnityEngine;

public class AgentStatHealthSystem : AgentHealthSystem
{
    [Header("Config")]
    [SerializeField] private float _vitalityToHealthRatio;
    [SerializeField] private float _initialHealth;

    [Header("Debug Fields")]
    [SerializeField] private AgentStats _agentStats;

    public override void Initialize()
    {
        base.Initialize();

        _agentStats = AgentCore.GetAgentComponent<AgentStats>();
        MaxHealth = CalculateMaxHealth();
        CurrentHealth = MaxHealth;
    }

    private float CalculateMaxHealth()
    {
        float vitalityValue = _agentStats.GetCurrentStatValue(EStats.Vitality);

        return _initialHealth + (vitalityValue * _vitalityToHealthRatio);
    }
}
