using System.Collections.Generic;

namespace Youregone.BehaviourTrees
{
    public class AnyAbilityOffCooldownCondition : ConditionNode
    {
        private AgentAbilitySystem _agentAbilitySystem;

        public AnyAbilityOffCooldownCondition(AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
        {
            _agentAbilitySystem = agentAbilitySystem;
        }

        public override ENodeState Evaluate()
        {
            foreach (KeyValuePair<string, Ability> keyValuePair in _agentAbilitySystem.AbilityDictionary)
            {
                if (!keyValuePair.Value.OnCooldown)
                    return ENodeState.Success;
            }

            return ENodeState.Failure;
        }

        protected override bool Predicate()
        {
            foreach (KeyValuePair<string, Ability> keyValuePair in _agentAbilitySystem.AbilityDictionary)
            {
                if (!keyValuePair.Value.OnCooldown)
                    return true;
            }

            return false;
        }
    }
}