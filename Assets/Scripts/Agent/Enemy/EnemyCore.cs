using UnityEngine;

public class EnemyCore : AgentCoreBase
{
    [Header("Config")]
    [SerializeField] private EnemyStateMachine _enemyStateMachine;

    [Header("Agent Components")]
    [SerializeField] protected AgentTargetDetectionZone _agentTargetDetectionZone;

    public EnemyStateMachine GetEnemyStateMachine() => _enemyStateMachine;

    protected override void InitializeComponentList()
    {
        _agentComponents.Add(_agentTargetDetectionZone);

        base.InitializeComponentList();
    }

    protected override void HealthSystem_OnDeath()
    {
        if (_enemyStateMachine.DisableOnDeath)
            _enemyStateMachine.enabled = false;

        foreach (AgentMonobehaviourComponent agentComponent in _disableOnDeathComponents)
        {
            agentComponent.DisableComponent();
        }
    }
}
