using UnityEngine;

public class CastDashFromTarget : CastDashToTargetNode
{
    public CastDashFromTarget(BaseEnemyBehaviour enemyBehaviour,
                              string abilityName,
                              AgentAbilitySystem agentAbilitySystem,
                              int nodePriority = 0) : base(enemyBehaviour, abilityName, agentAbilitySystem, nodePriority)
    {}

    public override ENodeState Evaluate()
    {
        Vector2 dashFromTargetPosition = (_enemyBehaviour.transform.position - _enemyBehaviour.GetCurrentTargetTransform().position).normalized;
        _agentAbilitySystem.CastAbility(_abilityName, dashFromTargetPosition);
        return ENodeState.Running;
    }
}
