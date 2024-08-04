
namespace Youregone.BehaviourTrees
{
    public class AbilityOffCooldownCondition : ConditionNode
    {
        private string _abilityName;
        private AgentAbilitySystem _agentAbilitySystem;

        public AbilityOffCooldownCondition(AgentAbilitySystem agentAbilitySystem, string abilityName, int nodePriority = 0) : base(nodePriority)
        {
            _agentAbilitySystem = agentAbilitySystem;
            _abilityName = abilityName;
        }

        protected override bool Predicate()
        {
            return !_agentAbilitySystem.IsOnCooldown(_abilityName);
        }
    }
}