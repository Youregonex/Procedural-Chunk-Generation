
namespace Youregone.BehaviourTrees
{
    public class AgentSpawnedCondition : ConditionNode
    {
        private BaseEnemyBehaviour _enemyBehaviour;

        public AgentSpawnedCondition(BaseEnemyBehaviour enemyBehaviour, int nodePriority = 0) : base(nodePriority)
        {
            _enemyBehaviour = enemyBehaviour;
        }

        protected override bool Predicate()
        {
            return _enemyBehaviour.IsSpawned;
        }
    }
}