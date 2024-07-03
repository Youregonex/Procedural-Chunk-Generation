using UnityEngine;

public class EnemyCore : AgentCoreBase
{
    [Header("Agent Components")]
    [SerializeField] protected AgentTargetDetectionZone _agentTargetDetectionZone;
    [SerializeField] private BaseEnemyBehaviour _enemyBehaviour;

    protected override void InitializeComponentList()
    {
        _agentComponents.Add(_agentTargetDetectionZone);
        _agentComponents.Add(_enemyBehaviour);

        base.InitializeComponentList();
    }
}
