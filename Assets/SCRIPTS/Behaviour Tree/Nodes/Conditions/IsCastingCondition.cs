
namespace Youregone.BehaviourTrees
{
    public class IsCastingCondition : ConditionNode
    {
        private AgentAbilitySystem _agentAbilitySystem;

        public IsCastingCondition(AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
        {
            _agentAbilitySystem = agentAbilitySystem;
        }

        protected override bool Predicate()
        {
            return _agentAbilitySystem.IsCastingAbility;
        }
    }
}