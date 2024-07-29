using System.Collections.Generic;
using UnityEngine;

public class CastFirstOffCooldownAbilityNode : Node
{
    private AgentAbilitySystem _agentAbilitySystem;

    public CastFirstOffCooldownAbilityNode(AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
    {
        _agentAbilitySystem = agentAbilitySystem;
    }

    public override ENodeState Evaluate()
    {
        foreach (KeyValuePair<string, Ability> keyValuePair in _agentAbilitySystem.AbilityDictionary)
        {
            if (!keyValuePair.Value.OnCooldown)
            {
                _agentAbilitySystem.CastAbility(keyValuePair.Value.Name.ToUpper(), Vector2.zero);
                break;
            }
        }

        return ENodeState.Running;
    }
}
