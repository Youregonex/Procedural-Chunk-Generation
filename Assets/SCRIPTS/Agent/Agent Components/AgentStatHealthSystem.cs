using UnityEngine;

public class AgentStatHealthSystem : AgentHealthSystem
{
    [Header("Config")]
    [SerializeField] private float _vitalityToHealthRatio;
    [SerializeField] private float _initialHealth;

    [Header("Debug Fields")]
    [SerializeField] private AgentStats _agentStats;


    protected override void Start()
    {
        _agentCore = GetComponent<AgentCoreBase>();
        _agentStats = _agentCore.GetAgentComponent<AgentStats>();

        MaxHealth = CalculateMaxHealth();
        CurrentHealth = MaxHealth;

        base.Start();
    }

    private float CalculateMaxHealth()
    {
        float vitalityValue = _agentStats.GetCurrentStatValue(EStats.Vitality);

        return _initialHealth + (vitalityValue * _vitalityToHealthRatio);
    }
}
