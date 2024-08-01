
namespace Youregone.BehaviourTrees
{
    public class IsCastingCondition : Node
    {
        private AgentAbilitySystem _agentAbilitySystem;

        public IsCastingCondition(AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
        {
            _agentAbilitySystem = agentAbilitySystem;
        }

        public override ENodeState Evaluate()
        {
            _nodeState = _agentAbilitySystem.IsCastingAbility ? ENodeState.Success : ENodeState.Failure;

            return _nodeState;
        }
    }

}