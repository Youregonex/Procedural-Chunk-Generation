
namespace Youregone.BehaviourTrees
{
    public class CastDashToTargetNode : BehaviourNode
    {
        protected string _abilityName;
        protected AgentAbilitySystem _agentAbilitySystem;
        protected BaseEnemyBehaviour _enemyBehaviour;

        public CastDashToTargetNode(BaseEnemyBehaviour enemyBehaviour, string abilityName, AgentAbilitySystem agentAbilitySystem, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
            _abilityName = abilityName;
            _agentAbilitySystem = agentAbilitySystem;
        }

        public override ENodeState Evaluate()
        {
            _agentAbilitySystem.CastAbility(_abilityName, _enemyBehaviour.MovementDirection);
            return ENodeState.Running;
        }
    }

}